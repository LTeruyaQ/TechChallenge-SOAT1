using Aplicacao.Interfaces.Servicos;
using Aplicacao.Notificacoes.OS;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOSTests.Notificacoes
{
    public class OrdemServicoCanceladaHandlerTests
    {
        private readonly Mock<IRepositorio<InsumoOS>> _repositorioMock;
        private readonly Mock<IInsumoOSServico> _insumoOSServicoMock;
        private readonly Mock<ILogServico<OrdemServicoCanceladaHandler>> _logServicoMock;
        private readonly OrdemServicoCanceladaHandler _handler;

        public OrdemServicoCanceladaHandlerTests()
        {
            _repositorioMock = new Mock<IRepositorio<InsumoOS>>();
            _insumoOSServicoMock = new Mock<IInsumoOSServico>();
            _logServicoMock = new Mock<ILogServico<OrdemServicoCanceladaHandler>>();
            _handler = new OrdemServicoCanceladaHandler(
                _repositorioMock.Object,
                _insumoOSServicoMock.Object,
                _logServicoMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveChamarDevolverInsumos_QuandoHaInsumos()
        {
            // Arrange
            var notification = new OrdemServicoCanceladaEvent(System.Guid.NewGuid());
            var insumos = new List<InsumoOS> { new InsumoOS() };
            _repositorioMock.Setup(r => r.ListarSemRastreamentoAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<InsumoOS>>())).ReturnsAsync(insumos);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _insumoOSServicoMock.Verify(s => s.DevolverInsumosAoEstoqueAsync(insumos), Times.Once);
        }

        [Fact]
        public async Task Handle_NaoDeveChamarDevolverInsumos_QuandoNaoHaInsumos()
        {
            // Arrange
            var notification = new OrdemServicoCanceladaEvent(System.Guid.NewGuid());
            _repositorioMock.Setup(r => r.ListarSemRastreamentoAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<InsumoOS>>())).ReturnsAsync(new List<InsumoOS>());

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _insumoOSServicoMock.Verify(s => s.DevolverInsumosAoEstoqueAsync(It.IsAny<IEnumerable<InsumoOS>>()), Times.Never);
        }
    }
}
