using Aplicacao.Notificacoes.OS;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOSTests.Notificacoes
{
    public class OrdemServicoFinalizadaHandlerTests
    {
        private readonly Mock<IRepositorio<OrdemServico>> _repositorioMock;
        private readonly Mock<ILogServico<OrdemServicoFinalizadaHandler>> _logServicoMock;
        private readonly Mock<IServicoEmail> _emailServicoMock;
        private readonly OrdemServicoFinalizadaHandler _handler;

        public OrdemServicoFinalizadaHandlerTests()
        {
            _repositorioMock = new Mock<IRepositorio<OrdemServico>>();
            _logServicoMock = new Mock<ILogServico<OrdemServicoFinalizadaHandler>>();
            _emailServicoMock = new Mock<IServicoEmail>();

            // Setup for the HTML template
            var templateDir = Path.Combine(System.AppContext.BaseDirectory, "Templates");
            if (!Directory.Exists(templateDir))
            {
                Directory.CreateDirectory(templateDir);
            }
            File.WriteAllText(Path.Combine(templateDir, "EmailOrdemServicoFinalizada.html"), "<html><body>{{NOME_CLIENTE}}</body></html>");

            _handler = new OrdemServicoFinalizadaHandler(
                _repositorioMock.Object,
                _logServicoMock.Object,
                _emailServicoMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveEnviarEmail_QuandoOsEncontrada()
        {
            // Arrange
            var notification = new OrdemServicoFinalizadaEvent(System.Guid.NewGuid());
            var os = new OrdemServico
            {
                Cliente = new Cliente { Nome = "Teste", Contato = new Contato { Email = "test@test.com" } },
                Servico = new Servico { Nome = "Teste", Descricao = "Teste" },
                Veiculo = new Veiculo { Modelo = "Teste", Placa = "ABC-1234" }
            };
            _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<OrdemServico>>())).ReturnsAsync(os);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailServicoMock.Verify(s => s.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NaoDeveEnviarEmail_QuandoOsNaoEncontrada()
        {
            // Arrange
            var notification = new OrdemServicoFinalizadaEvent(System.Guid.NewGuid());
            _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<OrdemServico>>())).ReturnsAsync((OrdemServico)null);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailServicoMock.Verify(s => s.EnviarAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
