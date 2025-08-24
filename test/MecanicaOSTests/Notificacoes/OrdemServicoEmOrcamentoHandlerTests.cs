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
    public class OrdemServicoEmOrcamentoHandlerTests
    {
        private readonly Mock<IRepositorio<OrdemServico>> _repositorioMock;
        private readonly Mock<IRepositorio<Orcamento>> _orcamentoRepositorioMock;
        private readonly Mock<IServicoEmail> _emailServicoMock;
        private readonly Mock<ILogServico<OrdemServicoEmOrcamentoHandler>> _logServicoMock;
        private readonly Mock<IUnidadeDeTrabalho> _udtMock;
        private readonly OrdemServicoEmOrcamentoHandler _handler;

        public OrdemServicoEmOrcamentoHandlerTests()
        {
            _repositorioMock = new Mock<IRepositorio<OrdemServico>>();
            _orcamentoRepositorioMock = new Mock<IRepositorio<Orcamento>>();
            _emailServicoMock = new Mock<IServicoEmail>();
            _logServicoMock = new Mock<ILogServico<OrdemServicoEmOrcamentoHandler>>();
            _udtMock = new Mock<IUnidadeDeTrabalho>();

            // Setup for the HTML template
            var templateDir = Path.Combine(System.AppContext.BaseDirectory, "Templates");
            if (!Directory.Exists(templateDir))
            {
                Directory.CreateDirectory(templateDir);
            }
            File.WriteAllText(Path.Combine(templateDir, "EmailOrcamentoOS.html"), "<html><body>{{NOME_CLIENTE}}</body></html>");

            _handler = new OrdemServicoEmOrcamentoHandler(
                _repositorioMock.Object,
                _orcamentoRepositorioMock.Object,
                _emailServicoMock.Object,
                _logServicoMock.Object,
                _udtMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveEnviarEmailEAtualizarOS()
        {
            // Arrange
            var notification = new OrdemServicoEmOrcamentoEvent(System.Guid.NewGuid());
            Guid osId = Guid.NewGuid();

            var os = new OrdemServico
            {
                Id = osId,
                Cliente = new Cliente { Nome = "Teste", Contato = new Contato { Email = "test@test.com" } },
                Servico = new Servico { Nome = "Teste", Descricao = "Teste", Valor = 100 },
            };

            _repositorioMock.Setup(r => r.ObterUmAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<OrdemServico>>())).ReturnsAsync(os);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailServicoMock.Verify(s => s.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _repositorioMock.Verify(r => r.EditarAsync(os), Times.Once);
            _udtMock.Verify(u => u.Commit(), Times.Exactly(2));
        }
    }
}
