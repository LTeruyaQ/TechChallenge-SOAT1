using Aplicacao.Interfaces.Servicos;
using Aplicacao.Notificacoes.OS;
using Dominio.Entidades;
using Dominio.Especificacoes.Insumo;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;
using Xunit;

namespace MecanicaOSTests.Servicos.Notificacoes;

public class OrdemServicoCanceladaHandlerTests
{
    private readonly Mock<IRepositorio<InsumoOS>> _insumoOSRepositorioMock;
    private readonly Mock<IInsumoOSServico> _insumoOSServicoMock;
    private readonly Mock<ILogServico<OrdemServicoCanceladaHandler>> _logServicoMock;
    private readonly OrdemServicoCanceladaHandler _handler;
    private readonly Guid _ordemServicoId = Guid.NewGuid();

    public OrdemServicoCanceladaHandlerTests()
    {
        _insumoOSRepositorioMock = new Mock<IRepositorio<InsumoOS>>();
        _insumoOSServicoMock = new Mock<IInsumoOSServico>();
        _logServicoMock = new Mock<ILogServico<OrdemServicoCanceladaHandler>>();
        
        _handler = new OrdemServicoCanceladaHandler(
            _insumoOSRepositorioMock.Object, 
            _insumoOSServicoMock.Object, 
            _logServicoMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoNaoEncontrarInsumos_NaoDeveChamarDevolverInsumosAoEstoque()
    {
        // Arrange
        var notificacao = new OrdemServicoCanceladaEvent(_ordemServicoId);
        var insumosVazios = new List<InsumoOS>();

        _insumoOSRepositorioMock
            .Setup(x => x.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .ReturnsAsync(insumosVazios);

        // Act
        await _handler.Handle(notificacao, CancellationToken.None);

        // Assert
        _insumoOSServicoMock.Verify(
            x => x.DevolverInsumosAoEstoqueAsync(It.IsAny<IEnumerable<InsumoOS>>()), 
            Times.Never);
            
        _logServicoMock.Verify(
            x => x.LogInicio(It.IsAny<string>(), _ordemServicoId), 
            Times.Once);
            
        // Quando não há insumos, o LogFim não deve ser chamado
        _logServicoMock.Verify(
            x => x.LogFim(It.IsAny<string>(), It.IsAny<object>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoEncontrarInsumos_DeveChamarDevolverInsumosAoEstoque()
    {
        // Arrange
        var notificacao = new OrdemServicoCanceladaEvent(_ordemServicoId);
        var insumos = new List<InsumoOS>
        {
            new() { Id = Guid.NewGuid(), OrdemServicoId = _ordemServicoId, Quantidade = 1 },
            new() { Id = Guid.NewGuid(), OrdemServicoId = _ordemServicoId, Quantidade = 2 }
        };

        _insumoOSRepositorioMock
            .Setup(x => x.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .ReturnsAsync(insumos);

        // Act
        await _handler.Handle(notificacao, CancellationToken.None);

        // Assert
        _insumoOSServicoMock.Verify(
            x => x.DevolverInsumosAoEstoqueAsync(It.Is<IEnumerable<InsumoOS>>(i => i.Count() == 2)), 
            Times.Once);
            
        _logServicoMock.Verify(
            x => x.LogInicio(It.IsAny<string>(), _ordemServicoId), 
            Times.Once);
            
        _logServicoMock.Verify(
            x => x.LogFim(It.IsAny<string>(), null), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoOcorrerErro_DeveLogarErroELancarExcecao()
    {
        // Arrange
        var notificacao = new OrdemServicoCanceladaEvent(_ordemServicoId);
        var exception = new Exception("Erro ao processar");

        _insumoOSRepositorioMock
            .Setup(x => x.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(notificacao, CancellationToken.None));
        
        _logServicoMock.Verify(
            x => x.LogErro(It.IsAny<string>(), It.Is<Exception>(e => e == exception)), 
            Times.Once);
    }
}
