using API.Controllers;
using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace MecanicaOSTests.API
{
    public class VeiculoControllerTests
    {
        private readonly Mock<IVeiculoServico> _veiculoServicoMock = new();
        private readonly Mock<ILogger<VeiculoController>> _loggerMock = new();

        private VeiculoController CriarController()
        {
            var controller = new VeiculoController(_veiculoServicoMock.Object, _loggerMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            return controller;
        }

        [Fact]
        public async Task Cadastrar_QuandoChamado_RetornaCreatedAtAction()
        {
            // Arrange
            var controller = CriarController();
            var request = new CadastrarVeiculoRequest();
            _veiculoServicoMock.Setup(x => x.CadastrarAsync(request)).ReturnsAsync(new Aplicacao.DTOs.Responses.Veiculo.VeiculoResponse { Id = Guid.NewGuid() });

            // Act
            var result = await controller.Cadastrar(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Deletar_QuandoChamado_RetornaNoContent()
        {
            // Arrange
            var controller = CriarController();
            _veiculoServicoMock.Setup(s => s.DeletarAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await controller.Deletar(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Editar_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            var request = new AtualizarVeiculoRequest();
            _veiculoServicoMock.Setup(x => x.AtualizarAsync(It.IsAny<Guid>(), request)).ReturnsAsync(new Aplicacao.DTOs.Responses.Veiculo.VeiculoResponse());


            // Act
            var result = await controller.Editar(Guid.NewGuid(), request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorCliente_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.ObterPorCliente(Guid.NewGuid());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorId_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
             _veiculoServicoMock.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Aplicacao.DTOs.Responses.Veiculo.VeiculoResponse());

            // Act
            var result = await controller.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.IsType<OkObjectResult>(result);
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
    }
}
