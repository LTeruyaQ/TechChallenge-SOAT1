using Aplicacao.Jobs;
using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Interfaces.Repositorios;
using FluentAssertions;
using Moq;
using Dominio.Interfaces.Servicos;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Especificacoes;

namespace MecanicaOSTests.Servicos;

public class VerificarOrcamentoExpiradoJobTests
{
    private readonly Mock<IRepositorio<OrdemServico>> _ordemServicoRepositorioMock;
    private readonly Mock<IInsumoOSServico> _insumoOSServicoMock;
    private readonly Mock<IUnidadeDeTrabalho> _unidadeTrabalhoMock;
    private readonly Mock<ILogServico<VerificarOrcamentoExpiradoJob>> _logServicoMock;
    private readonly VerificarOrcamentoExpiradoJob _job;
    private readonly Guid _ordemServicoId = Guid.NewGuid();
    private readonly Guid _clienteId = Guid.NewGuid();
    private readonly Guid _veiculoId = Guid.NewGuid();
    private readonly Guid _servicoId = Guid.NewGuid();
    private readonly Guid _insumoOSId = Guid.NewGuid();
    private readonly Guid _estoqueId = Guid.NewGuid();

    public VerificarOrcamentoExpiradoJobTests()
    {
        _ordemServicoRepositorioMock = new Mock<IRepositorio<OrdemServico>>();
        _insumoOSServicoMock = new Mock<IInsumoOSServico>();
        _unidadeTrabalhoMock = new Mock<IUnidadeDeTrabalho>();
        _logServicoMock = new Mock<ILogServico<VerificarOrcamentoExpiradoJob>>();
        
        _job = new VerificarOrcamentoExpiradoJob(
            _ordemServicoRepositorioMock.Object,
            _insumoOSServicoMock.Object,
            _unidadeTrabalhoMock.Object,
            _logServicoMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoNaoHaOrdensExpiradas_NaoDeveFazerNada()
    {
        // Arrange
        var ordensVazias = new List<OrdemServico>();
        _ordemServicoRepositorioMock
            .Setup(r => r.ListarAsync(It.IsAny<ObterOSOrcamentoExpiradoEspecificacao>()))
            .ReturnsAsync(ordensVazias);

        // Act
        await _job.ExecutarAsync();

        // Assert
        _ordemServicoRepositorioMock.Verify(r => r.ListarAsync(It.IsAny<ObterOSOrcamentoExpiradoEspecificacao>()), Times.Once);
        _insumoOSServicoMock.Verify(i => i.DevolverInsumosAoEstoqueAsync(It.IsAny<IEnumerable<InsumoOS>>()), Times.Never);
        _ordemServicoRepositorioMock.Verify(r => r.EditarVariosAsync(It.IsAny<IEnumerable<OrdemServico>>()), Times.Never);
        _unidadeTrabalhoMock.Verify(u => u.Commit(), Times.Never);
        _logServicoMock.Verify(l => l.LogInicio(It.Is<string>(s => s != null), It.Is<object?>(o => true)), Times.Once);
        _logServicoMock.Verify(l => l.LogFim(It.Is<string>(s => s != null), It.Is<object?>(o => true)), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoHaOrdensExpiradas_DeveAtualizarStatusEDevolverInsumos()
    {
        // Arrange
        var ordemServico = CriarOrdemServicoComOrcamentoExpirado();
        var ordensExpiradas = new List<OrdemServico> { ordemServico };

        _ordemServicoRepositorioMock
            .Setup(r => r.ListarAsync(It.IsAny<ObterOSOrcamentoExpiradoEspecificacao>()))
            .ReturnsAsync(ordensExpiradas);

        _ordemServicoRepositorioMock
            .Setup(r => r.EditarVariosAsync(It.IsAny<IEnumerable<OrdemServico>>()))
            .Returns(Task.CompletedTask);

        _unidadeTrabalhoMock
            .Setup(u => u.Commit())
            .ReturnsAsync(true);

        // Act
        await _job.ExecutarAsync();

        // Assert
        _ordemServicoRepositorioMock.Verify(r => r.ListarAsync(It.IsAny<ObterOSOrcamentoExpiradoEspecificacao>()), Times.Once);
        _insumoOSServicoMock.Verify(i => i.DevolverInsumosAoEstoqueAsync(It.IsAny<IEnumerable<InsumoOS>>()), Times.Once);
        _ordemServicoRepositorioMock.Verify(r => r.EditarVariosAsync(It.IsAny<IEnumerable<OrdemServico>>()), Times.Once);
        _unidadeTrabalhoMock.Verify(u => u.Commit(), Times.Once);
        _logServicoMock.Verify(l => l.LogInicio(It.Is<string>(s => s != null), It.Is<object?>(o => true)), Times.Once);
        _logServicoMock.Verify(l => l.LogFim(It.Is<string>(s => s != null), It.Is<object?>(o => true)), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoOcorrerExcecao_DeveLogarERelancarExcecao()
    {
        // Arrange
        var exception = new Exception("Erro ao processar ordens de serviço");
        _ordemServicoRepositorioMock
            .Setup(r => r.ListarAsync(It.IsAny<ObterOSOrcamentoExpiradoEspecificacao>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await _job.Invoking(j => j.ExecutarAsync())
            .Should().ThrowAsync<Exception>().WithMessage(exception.Message);

        _logServicoMock.Verify(l => l.LogErro(It.Is<string>(s => s != null), It.Is<Exception>(e => e == exception)), Times.Once);
    }

    private OrdemServico CriarOrdemServicoComOrcamentoExpirado()
    {
        var servico = new Servico 
        { 
            Id = _servicoId, 
            Nome = "Troca de Óleo", 
            Descricao = "Troca de óleo do motor",
            Valor = 150.00m,
            Disponivel = true
        };
        
        var estoque = new Estoque 
        { 
            Id = _estoqueId, 
            Insumo = "Óleo 5W30",
            Descricao = "Óleo sintético 5W30 1L",
            Preco = 50.00m,
            QuantidadeDisponivel = 10,
            QuantidadeMinima = 2
        };
        
        var insumoOS = new InsumoOS
        {
            Id = _insumoOSId,
            Estoque = estoque,
            EstoqueId = _estoqueId,
            Quantidade = 2
        };

        return new OrdemServico
        {
            Id = _ordemServicoId,
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Servico = servico,
            InsumosOS = [insumoOS],
            Status = StatusOrdemServico.AguardandoAprovacao,
            DataEnvioOrcamento = DateTime.UtcNow.AddDays(-4) // Orçamento expirado (mais de 3 dias)
        };
    }
}
