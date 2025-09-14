using API.Controllers;
using MecanicaOSTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MecanicaOSTests.API.Controllers
{
    public class ClienteControllerTests
    {
        private readonly Mock<IClienteServico> _clienteServicoMock;
        private readonly ClienteController _controller;

        public ClienteControllerTests()
        {
            _clienteServicoMock = new Mock<IClienteServico>();
            _controller = new ClienteController(_clienteServicoMock.Object);
        }

        [Fact]
        public async Task ObterPorId_QuandoClienteExistir_DeveRetornarOkComCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteResponseDto = new ClienteResponse { Id = clienteId, Nome = "Teste" };
            _clienteServicoMock.Setup(s => s.ObterPorIdAsync(clienteId)).ReturnsAsync(clienteResponseDto);

            // Act
            var resultado = await _controller.ObterPorId(clienteId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var clienteRetornado = Assert.IsType<ClienteResponse>(okResult.Value);
            Assert.Equal(clienteResponseDto.Id, clienteRetornado.Id);
        }

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var cadastrarClienteDto = ClienteFixture.CriarCadastrarClienteRequestValido();
            var clienteResponseDto = new ClienteResponse { Id = Guid.NewGuid() };
            _clienteServicoMock.Setup(s => s.CadastrarAsync(cadastrarClienteDto)).ReturnsAsync(clienteResponseDto);

            // Act
            var resultado = await _controller.Criar(cadastrarClienteDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPorId), createdAtActionResult.ActionName);
            Assert.Equal(clienteResponseDto.Id, ((ClienteResponse)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task Atualizar_QuandoClienteExistir_DeveRetornarOk()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var atualizarClienteDto = ClienteFixture.CriarAtualizarClienteRequestValido();
            var clienteResponseDto = new ClienteResponse { Id = clienteId };
            _clienteServicoMock.Setup(s => s.AtualizarAsync(clienteId, atualizarClienteDto)).ReturnsAsync(clienteResponseDto);

            // Act
            var resultado = await _controller.Atualizar(clienteId, atualizarClienteDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsType<ClienteResponse>(okResult.Value);
        }

        [Fact]
        public async Task Remover_QuandoClienteExistir_DeveRetornarNoContent()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            _clienteServicoMock.Setup(s => s.RemoverAsync(clienteId)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.Remover(clienteId);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeClientes()
        {
            // Arrange
            var clientes = new List<ClienteResponse> { new ClienteResponse() };
            _clienteServicoMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(clientes);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<ClienteResponse>>(okResult.Value);
        }
    }
}
