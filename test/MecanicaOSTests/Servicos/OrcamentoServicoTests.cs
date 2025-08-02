using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Servicos;

public class OrcamentoServicoTests
{
    private readonly Mock<ILogServico<OrcamentoServico>> _logServicoMock;
    private readonly OrcamentoServico _orcamentoServico;
    private readonly Guid _servicoId = Guid.NewGuid();
    private readonly Guid _estoqueId = Guid.NewGuid();
    private readonly Guid _ordemServicoId = Guid.NewGuid();
    private readonly Guid _clienteId = Guid.NewGuid();
    private readonly Guid _veiculoId = Guid.NewGuid();

    public OrcamentoServicoTests()
    {
        _logServicoMock = new Mock<ILogServico<OrcamentoServico>>();
        _orcamentoServico = new OrcamentoServico(_logServicoMock.Object);
    }

    [Fact]
    public void GerarOrcamento_QuandoOrdemServicoValida_DeveRetornarValorTotalCorreto()
    {
        // Arrange
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
            Id = Guid.NewGuid(),
            Estoque = estoque,
            Quantidade = 2
        };

        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Servico = servico,
            InsumosOS = [insumoOS],
            Status = StatusOrdemServico.AguardandoAprovacao
        };

        // Act
        var resultado = _orcamentoServico.GerarOrcamento(ordemServico);

        // Assert
        resultado.Should().Be(250.00m); // 150 (serviço) + (2 * 50) (insumos)
        _logServicoMock.Verify(l => l.LogInicio(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        _logServicoMock.Verify(l => l.LogFim(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void GerarOrcamento_QuandoOrdemServicoSemInsumos_DeveRetornarApenasValorServico()
    {
        // Arrange
        var servico = new Servico 
        { 
            Id = _servicoId,
            Nome = "Revisão Básica", 
            Descricao = "Revisão básica do veículo",
            Valor = 200.00m,
            Disponivel = true
        };
        
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Servico = servico,
            InsumosOS = [],
            Status = StatusOrdemServico.AguardandoAprovacao
        };

        // Act
        var resultado = _orcamentoServico.GerarOrcamento(ordemServico);

        // Assert
        resultado.Should().Be(200.00m);
        _logServicoMock.Verify(l => l.LogInicio(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        _logServicoMock.Verify(l => l.LogFim(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void GerarOrcamento_QuandoOcorrerExcecao_DeveLogarERelancarExcecao()
    {
        // Arrange
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Servico = null!,
            InsumosOS = [],
            Status = StatusOrdemServico.AguardandoAprovacao
        };

        // Act & Assert
        _orcamentoServico.Invoking(s => s.GerarOrcamento(ordemServico))
            .Should().Throw<NullReferenceException>();

        _logServicoMock.Verify(l => l.LogErro(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }
}
