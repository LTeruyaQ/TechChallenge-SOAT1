using API.Controllers;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MecanicaOSTests.API
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioServico> _usuarioServicoMock = new();

        private UsuarioController CriarController()
        {
            var controller = new UsuarioController(_usuarioServicoMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            return controller;
        }

        [Fact]
        public async Task ObterTodos_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.ObterTodos();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorId_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            _usuarioServicoMock.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Aplicacao.DTOs.Responses.Usuario.UsuarioResponse());

            // Act
            var result = await controller.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Criar_QuandoChamado_RetornaCreatedAtAction()
        {
            // Arrange
            var controller = CriarController();
            var request = new CadastrarUsuarioRequest();
            _usuarioServicoMock.Setup(x => x.CadastrarAsync(request)).ReturnsAsync(new Aplicacao.DTOs.Responses.Usuario.UsuarioResponse { Id = Guid.NewGuid() });

            // Act
            var result = await controller.Criar(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Atualizar_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            var request = new AtualizarUsuarioRequest();

            // Act
            var result = await controller.Atualizar(Guid.NewGuid(), request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Deletar_QuandoChamado_RetornaNoContent()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.Deletar(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
