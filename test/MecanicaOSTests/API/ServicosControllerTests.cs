using API.Controllers;
using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MecanicaOSTests.API
{
    public class ServicosControllerTests
    {
        private readonly Mock<IServicoServico> _servicoServicoMock = new();

        private ServicosController CriarController()
        {
            var controller = new ServicosController(_servicoServicoMock.Object);
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
        public async Task ObterServicosDisponiveis_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.ObterServicosDisponiveis();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorId_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();

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
            var request = new CadastrarServicoRequest
            {
                Nome = "Teste",
                Descricao = "Teste",
                Valor = 10,
                Disponivel = true
            };
            _servicoServicoMock.Setup(x => x.CadastrarServicoAsync(request)).ReturnsAsync(new Aplicacao.DTOs.Responses.Servico.ServicoResponse { Id = Guid.NewGuid() });

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
            var request = new EditarServicoRequest
            {
                Nome = "Teste",
                Descricao = "Teste",
                Valor = 10,
                Disponivel = true
            };

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
