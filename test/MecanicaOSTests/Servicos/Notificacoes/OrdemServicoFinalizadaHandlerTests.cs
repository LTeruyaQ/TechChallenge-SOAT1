using Aplicacao.Interfaces.Servicos;
using Aplicacao.Notificacoes.OS;
using Dominio.Entidades;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;
using Xunit;

namespace MecanicaOSTests.Servicos.Notificacoes;

public class OrdemServicoFinalizadaHandlerTests
{
    private readonly Mock<IRepositorio<OrdemServico>> _ordemServicoRepositorioMock;
    private readonly Mock<ILogServico<OrdemServicoFinalizadaHandler>> _logServicoMock;
    private readonly Mock<IServicoEmail> _emailServicoMock;
    private readonly OrdemServicoFinalizadaHandler _handler;
    private readonly Guid _ordemServicoId = Guid.NewGuid();

    public OrdemServicoFinalizadaHandlerTests()
    {
        _ordemServicoRepositorioMock = new Mock<IRepositorio<OrdemServico>>();
        _logServicoMock = new Mock<ILogServico<OrdemServicoFinalizadaHandler>>();
        _emailServicoMock = new Mock<IServicoEmail>();
        
        _handler = new OrdemServicoFinalizadaHandler(
            _ordemServicoRepositorioMock.Object,
            _logServicoMock.Object,
            _emailServicoMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoNaoEncontrarOrdemServico_NaoDeveEnviarEmail()
    {
        // Arrange
        var notificacao = new OrdemServicoFinalizadaEvent(_ordemServicoId);
        
        _ordemServicoRepositorioMock
            .Setup(x => x.ObterUmSemRastreamentoAsync(It.IsAny<ObterOrdemServicoPorIdComIncludeEspecificacao>()))
            .ReturnsAsync((OrdemServico)null);

        // Act
        await _handler.Handle(notificacao, CancellationToken.None);

        // Assert
        _emailServicoMock.Verify(
            x => x.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
            
        _logServicoMock.Verify(
            x => x.LogInicio(It.IsAny<string>(), _ordemServicoId),
            Times.Once);
            
        _logServicoMock.Verify(
            x => x.LogFim(It.IsAny<string>(), It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoEncontrarOrdemServico_DeveEnviarEmailComDadosCorretos()
    {
        // Arrange
        var notificacao = new OrdemServicoFinalizadaEvent(_ordemServicoId);
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            Cliente = new Cliente 
            { 
                Nome = "João Silva", 
                Contato = new Contato { Email = "joao@teste.com" } 
            },
            Servico = new Servico { Nome = "Troca de Óleo", Descricao = "Troca de óleo do motor" },
            Veiculo = new Veiculo { Modelo = "Gol 1.0", Placa = "ABC1D23" }
        };

        _ordemServicoRepositorioMock
            .Setup(x => x.ObterUmSemRastreamentoAsync(It.IsAny<ObterOrdemServicoPorIdComIncludeEspecificacao>()))
            .ReturnsAsync(ordemServico);

        string conteudoEmailEnviado = string.Empty;
        _emailServicoMock
            .Setup(x => x.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<IEnumerable<string>, string, string>((emails, assunto, conteudo) => conteudoEmailEnviado = conteudo)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notificacao, CancellationToken.None);

        // Assert
        _emailServicoMock.Verify(
            x => x.EnviarAsync(
                It.Is<IEnumerable<string>>(e => e.Contains("joao@teste.com")),
                "Serviço Finalizado",
                It.IsAny<string>()),
            Times.Once);
            
        _logServicoMock.Verify(
            x => x.LogInicio(It.IsAny<string>(), _ordemServicoId),
            Times.Once);
            
        _logServicoMock.Verify(
            x => x.LogFim(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once);
            
        // Verifica se o conteúdo do email contém as informações esperadas
        Assert.Contains("João Silva", conteudoEmailEnviado);
        Assert.Contains("Troca de Óleo", conteudoEmailEnviado);
        Assert.Contains("Gol 1.0", conteudoEmailEnviado);
        Assert.Contains("ABC1D23", conteudoEmailEnviado);
    }

    [Fact]
    public async Task Handle_QuandoOcorrerErro_DeveLogarErroELancarExcecao()
    {
        // Arrange
        var notificacao = new OrdemServicoFinalizadaEvent(_ordemServicoId);
        var exception = new Exception("Erro ao processar");

        _ordemServicoRepositorioMock
            .Setup(x => x.ObterUmSemRastreamentoAsync(It.IsAny<ObterOrdemServicoPorIdComIncludeEspecificacao>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(notificacao, CancellationToken.None));
        
        _logServicoMock.Verify(
            x => x.LogErro(It.IsAny<string>(), It.Is<Exception>(e => e == exception)), 
            Times.Once);
    }
}
