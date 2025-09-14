using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace MecanicaOSTests.API.Controllers
{
    public class UsuarioLogadoControllerTests
    {
        private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServicoMock;
        private readonly UsuarioLogadoController _controller;

        public UsuarioLogadoControllerTests()
        {
            _usuarioLogadoServicoMock = new Mock<IUsuarioLogadoServico>();
            _controller = new UsuarioLogadoController(_usuarioLogadoServicoMock.Object);
        }

        [Fact]
        public void ObterDadosUsuarioLogado_QuandoAutenticado_DeveRetornarOkComDados()
        {
            // Arrange
            var usuario = new Usuario { Id = Guid.NewGuid(), Email = "teste@teste.com", TipoUsuario = TipoUsuario.Admin };
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "teste@teste.com") };
            _usuarioLogadoServicoMock.Setup(s => s.EstaAutenticado).Returns(true);
            _usuarioLogadoServicoMock.Setup(s => s.ObterUsuarioLogado()).Returns(usuario);
            _usuarioLogadoServicoMock.Setup(s => s.ObterTodasClaims()).Returns(claims);

            // Act
            var resultado = _controller.ObterDadosUsuarioLogado();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void ObterDadosUsuarioLogado_QuandoNaoAutenticado_DeveRetornarUnauthorized()
        {
            // Arrange
            _usuarioLogadoServicoMock.Setup(s => s.EstaAutenticado).Returns(false);

            // Act
            var resultado = _controller.ObterDadosUsuarioLogado();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(resultado);
        }

        [Fact]
        public void VerificarPermissoes_QuandoAutenticado_DeveRetornarOkComPermissoes()
        {
            // Arrange
            _usuarioLogadoServicoMock.Setup(s => s.EstaAutenticado).Returns(true);
            _usuarioLogadoServicoMock.Setup(s => s.PossuiPermissao("cliente")).Returns(true);
            _usuarioLogadoServicoMock.Setup(s => s.PossuiPermissao("administrador")).Returns(false);

            // Act
            var resultado = _controller.VerificarPermissoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void VerificarPermissoes_QuandoNaoAutenticado_DeveRetornarUnauthorized()
        {
            // Arrange
            _usuarioLogadoServicoMock.Setup(s => s.EstaAutenticado).Returns(false);

            // Act
            var resultado = _controller.VerificarPermissoes();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(resultado);
        }
    }
}
