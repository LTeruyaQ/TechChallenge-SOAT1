using Moq;

namespace MecanicaOSTests.Jobs
{
    public class VerificarEstoqueJobTests
    {
        private readonly Mock<IRepositorio<Estoque>> _estoqueRepositorioMock;
        private readonly Mock<IRepositorio<Usuario>> _usuarioRepositorioMock;
        private readonly Mock<IRepositorio<AlertaEstoque>> _alertaEstoqueRepositorioMock;
        private readonly Mock<IServicoEmail> _servicoEmailMock;
        private readonly Mock<ILogServico<VerificarEstoqueJob>> _logServicoMock;
        private readonly Mock<IUnidadeDeTrabalho> _udtMock;
        private readonly VerificarEstoqueJob _verificarEstoqueJob;

        public VerificarEstoqueJobTests()
        {
            _estoqueRepositorioMock = new Mock<IRepositorio<Estoque>>();
            _usuarioRepositorioMock = new Mock<IRepositorio<Usuario>>();
            _alertaEstoqueRepositorioMock = new Mock<IRepositorio<AlertaEstoque>>();
            _servicoEmailMock = new Mock<IServicoEmail>();
            _logServicoMock = new Mock<ILogServico<VerificarEstoqueJob>>();
            _udtMock = new Mock<IUnidadeDeTrabalho>();

            // Setup for the HTML template
            var templateDir = Path.Combine(System.AppContext.BaseDirectory, "Templates");
            if (!Directory.Exists(templateDir))
            {
                Directory.CreateDirectory(templateDir);
            }
            File.WriteAllText(Path.Combine(templateDir, "EmailAlertaEstoque.html"), "<html><body>{{INSUMOS}}</body></html>");


            _verificarEstoqueJob = new VerificarEstoqueJob(
                _estoqueRepositorioMock.Object,
                _usuarioRepositorioMock.Object,
                _alertaEstoqueRepositorioMock.Object,
                _servicoEmailMock.Object,
                _logServicoMock.Object,
                _udtMock.Object
            );
        }

        [Fact]
        public async Task ExecutarAsync_DeveEnviarEmail_QuandoHaInsumosCriticos()
        {
            // Arrange
            var insumosCriticos = new List<Estoque> { new Estoque { Id = System.Guid.NewGuid(), Insumo = "Parafuso", QuantidadeDisponivel = 1, QuantidadeMinima = 5 } };
            _estoqueRepositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<Estoque>>())).ReturnsAsync(insumosCriticos);
            _alertaEstoqueRepositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<AlertaEstoque>>())).ReturnsAsync(new List<AlertaEstoque>());
            _usuarioRepositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<Usuario>>())).ReturnsAsync(new List<Usuario> { new Usuario() });
            _udtMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            await _verificarEstoqueJob.ExecutarAsync();

            // Assert
            _servicoEmailMock.Verify(s => s.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _alertaEstoqueRepositorioMock.Verify(r => r.CadastrarVariosAsync(It.IsAny<IEnumerable<AlertaEstoque>>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_NaoDeveFazerNada_QuandoNaoHaInsumosCriticos()
        {
            // Arrange
            _estoqueRepositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<Estoque>>())).ReturnsAsync(new List<Estoque>());

            // Act
            await _verificarEstoqueJob.ExecutarAsync();

            // Assert
            _servicoEmailMock.Verify(s => s.EnviarAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
