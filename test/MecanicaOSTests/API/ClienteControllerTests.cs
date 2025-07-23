using API.Controllers;
using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.Interfaces.Servicos;
using Dominio.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MecanicaOSTests.API
{
    public class ClienteControllerTests
    {
        private readonly Mock<IClienteServico> _clienteServicoMock = new();

        private ClienteController CriarController()
        {
            var controller = new ClienteController(_clienteServicoMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            return controller;
        }

        [Fact]
        public async Task Post_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            var request = new CadastrarClienteRequest();

            // Act
            var result = await controller.Post(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Put_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            var request = new AtualizarClienteRequest();

            // Act
            var result = await controller.Put(Guid.NewGuid(), request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_QuandoChamado_RetornaNoContent()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAll_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
