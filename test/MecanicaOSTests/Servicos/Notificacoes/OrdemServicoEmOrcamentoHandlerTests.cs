using Aplicacao.Interfaces.Servicos;
using Aplicacao.Notificacoes.OS;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;
using Xunit;

namespace MecanicaOSTests.Servicos.Notificacoes;

public class OrdemServicoEmOrcamentoHandlerTests
{
    private readonly Mock<IRepositorio<OrdemServico>> _ordemServicoRepositorioMock;
    private readonly Mock<IOrcamentoServico> _orcamentoServicoMock;
    private readonly Mock<IServicoEmail> _emailServicoMock;
    private readonly Mock<ILogServico<OrdemServicoEmOrcamentoHandler>> _logServicoMock;
    private readonly Mock<IUnidadeDeTrabalho> _uotMock;
    private readonly OrdemServicoEmOrcamentoHandler _handler;
    private readonly Guid _ordemServicoId = Guid.NewGuid();

    public OrdemServicoEmOrcamentoHandlerTests()
    {
        _ordemServicoRepositorioMock = new Mock<IRepositorio<OrdemServico>>();
        _orcamentoServicoMock = new Mock<IOrcamentoServico>();
        _emailServicoMock = new Mock<IServicoEmail>();
        _logServicoMock = new Mock<ILogServico<OrdemServicoEmOrcamentoHandler>>();
        _uotMock = new Mock<IUnidadeDeTrabalho>();
        
        _handler = new OrdemServicoEmOrcamentoHandler(
            _ordemServicoRepositorioMock.Object,
            _orcamentoServicoMock.Object,
            _emailServicoMock.Object,
            _logServicoMock.Object,
            _uotMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoNaoEncontrarOrdemServico_NaoDeveFazerNada()
    {
        // Arrange
        var notificacao = new OrdemServicoEmOrcamentoEvent(_ordemServicoId);
        
        _ordemServicoRepositorioMock
            .Setup(x => x.ObterUmAsync(It.IsAny<ObterOrdemServicoPorIdComIncludeEspecificacao>()))
            .ReturnsAsync((OrdemServico)null);

        // Act
        await _handler.Handle(notificacao, CancellationToken.None);

        // Assert
        _ordemServicoRepositorioMock.Verify(
            x => x.EditarAsync(It.IsAny<OrdemServico>()),
            Times.Never);
            
        _uotMock.Verify(
            x => x.Commit(),
            Times.Never);
            
        _logServicoMock.Verify(
            x => x.LogInicio(It.IsAny<string>(), _ordemServicoId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoEncontrarOrdemServico_DeveAtualizarEEnviarEmail()
    {
        // Arrange
        var notificacao = new OrdemServicoEmOrcamentoEvent(_ordemServicoId);
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            Status = StatusOrdemServico.EmDiagnostico,
            Cliente = new Cliente { Contato = new Contato { Email = "cliente@teste.com" } },
            Servico = new Servico { Nome = "Troca de Óleo", Descricao = "Troca de óleo do motor", Valor = 100.0m },
            InsumosOS = new List<InsumoOS>()
        };

        _ordemServicoRepositorioMock
            .Setup(x => x.ObterUmAsync(It.IsAny<ObterOrdemServicoPorIdComIncludeEspecificacao>()))
            .ReturnsAsync(ordemServico);
            
        _orcamentoServicoMock
            .Setup(x => x.GerarOrcamento(It.IsAny<OrdemServico>()))
            .Returns(150.0m);
            
        _emailServicoMock
            .Setup(x => x.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notificacao, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, ordemServico.Status);
        Assert.NotNull(ordemServico.DataEnvioOrcamento);
        
        _ordemServicoRepositorioMock.Verify(
            x => x.EditarAsync(It.Is<OrdemServico>(os => os.Id == _ordemServicoId)),
            Times.Once);
            
        _uotMock.Verify(
            x => x.Commit(),
            Times.Once);
            
        _emailServicoMock.Verify(
            x => x.EnviarAsync(
                It.Is<IEnumerable<string>>(e => e.Contains("cliente@teste.com")),
                "Orçamento de Serviço",
                It.IsAny<string>()),
            Times.Once);
            
        _logServicoMock.Verify(
            x => x.LogInicio(It.IsAny<string>(), _ordemServicoId),
            Times.Once);
            
        _logServicoMock.Verify(
            x => x.LogFim(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoOcorrerErro_DeveLogarErroELancarExcecao()
    {
        // Arrange
        var notificacao = new OrdemServicoEmOrcamentoEvent(_ordemServicoId);
        var exception = new Exception("Erro ao processar");

        _ordemServicoRepositorioMock
            .Setup(x => x.ObterUmAsync(It.IsAny<ObterOrdemServicoPorIdComIncludeEspecificacao>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(notificacao, CancellationToken.None));
        
        _logServicoMock.Verify(
            x => x.LogErro(It.IsAny<string>(), It.Is<Exception>(e => e == exception)), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_DeveChamarServicoDeEmailComConteudoCorreto()
    {
        // Arrange
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            Cliente = new Cliente { Nome = "João Silva", Contato = new Contato { Email = "joao@teste.com" } },
            Servico = new Servico { Nome = "Troca de Óleo", Descricao = "Troca de óleo do motor", Valor = 100.0m },
            Orcamento = 150.0m,
            InsumosOS = new List<InsumoOS>
            {
                new() 
                { 
                    Quantidade = 2, 
                    Estoque = new Estoque 
                    { 
                        Insumo = "Óleo 10W40", 
                        Preco = 25.0m 
                    } 
                }
            }
        };

        _ordemServicoRepositorioMock
            .Setup(x => x.ObterUmAsync(It.IsAny<ObterOrdemServicoPorIdComIncludeEspecificacao>()))
            .ReturnsAsync(ordemServico);
            
        _orcamentoServicoMock
            .Setup(x => x.GerarOrcamento(It.IsAny<OrdemServico>()))
            .Returns(150.0m);

        string conteudoEmail = string.Empty;
        _emailServicoMock
            .Setup(x => x.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<IEnumerable<string>, string, string>((emails, assunto, conteudo) => conteudoEmail = conteudo)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(new OrdemServicoEmOrcamentoEvent(_ordemServicoId), CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, ordemServico.Status);
        Assert.NotNull(ordemServico.DataEnvioOrcamento);
        
        _emailServicoMock.Verify(
            x => x.EnviarAsync(
                It.Is<IEnumerable<string>>(e => e.Contains("joao@teste.com")),
                "Orçamento de Serviço",
                It.IsAny<string>()),
            Times.Once);
            
        // Verifica se o conteúdo do email contém as informações esperadas
        Assert.Contains("João Silva", conteudoEmail);
        Assert.Contains("Troca de Óleo", conteudoEmail);
        Assert.Contains("100,00", conteudoEmail);
        Assert.Contains("150,00", conteudoEmail);
        Assert.Contains("Óleo 10W40 (2 und)", conteudoEmail);
        Assert.Contains("50,00", conteudoEmail); // 2 * 25,00
    }
}
