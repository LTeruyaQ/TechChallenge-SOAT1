using API.Controllers;
using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MecanicaOSTests.API
{
    public class EstoqueControllerTests
    {
        private readonly Mock<IEstoqueServico> _estoqueServicoMock = new();

        private EstoqueController CriarController()
        {
            var controller = new EstoqueController(_estoqueServicoMock.Object);
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
            _estoqueServicoMock.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Aplicacao.DTOs.Responses.Estoque.EstoqueResponse());

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
            var request = new CadastrarEstoqueRequest();
            _estoqueServicoMock.Setup(x => x.CadastrarAsync(request)).ReturnsAsync(new Aplicacao.DTOs.Responses.Estoque.EstoqueResponse { Id = Guid.NewGuid() });

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
            var request = new AtualizarEstoqueRequest();

            // Act
            var result = await controller.Atualizar(Guid.NewGuid(), request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Remover_QuandoChamado_RetornaNoContent()
        {
            // Arrange
            var controller = CriarController();
            _estoqueServicoMock.Setup(s => s.DeletarAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await controller.Remover(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
