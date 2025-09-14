using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MecanicaOSTests.API.Controllers
{
    public static class EstoqueFixture
    {
        public static CadastrarEstoqueRequest CriarCadastrarEstoqueRequestValido()
        {
            return new CadastrarEstoqueRequest
            {
                Insumo = "Oleo",
                Descricao = "Oleo de motor",
                Preco = 50,
                QuantidadeDisponivel = 10,
                QuantidadeMinima = 2
            };
        }

        public static AtualizarEstoqueRequest CriarAtualizarEstoqueRequestValido()
        {
            return new AtualizarEstoqueRequest
            {
                Descricao = "Oleo de motor atualizado",
                Preco = 55,
                QuantidadeDisponivel = 15,
                QuantidadeMinima = 3
            };
        }
    }

    public class EstoqueControllerTests
    {
        private readonly Mock<IEstoqueServico> _estoqueServicoMock;
        private readonly EstoqueController _controller;

        public EstoqueControllerTests()
        {
            _estoqueServicoMock = new Mock<IEstoqueServico>();
            _controller = new EstoqueController(_estoqueServicoMock.Object);
        }

        [Fact]
        public async Task ObterPorId_QuandoEstoqueExistir_DeveRetornarOkComEstoque()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueResponseDto = new EstoqueResponse { Id = estoqueId };
            _estoqueServicoMock.Setup(s => s.ObterPorIdAsync(estoqueId)).ReturnsAsync(estoqueResponseDto);

            // Act
            var resultado = await _controller.ObterPorId(estoqueId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var estoqueRetornado = Assert.IsType<EstoqueResponse>(okResult.Value);
            Assert.Equal(estoqueResponseDto.Id, estoqueRetornado.Id);
        }

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var cadastrarEstoqueDto = EstoqueFixture.CriarCadastrarEstoqueRequestValido();
            var estoqueResponseDto = new EstoqueResponse { Id = Guid.NewGuid() };
            _estoqueServicoMock.Setup(s => s.CadastrarAsync(cadastrarEstoqueDto)).ReturnsAsync(estoqueResponseDto);

            // Act
            var resultado = await _controller.Criar(cadastrarEstoqueDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPorId), createdAtActionResult.ActionName);
            Assert.Equal(estoqueResponseDto.Id, ((EstoqueResponse)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task Atualizar_QuandoEstoqueExistir_DeveRetornarOk()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var atualizarEstoqueDto = EstoqueFixture.CriarAtualizarEstoqueRequestValido();
            var estoqueResponseDto = new EstoqueResponse { Id = estoqueId };
            _estoqueServicoMock.Setup(s => s.AtualizarAsync(estoqueId, atualizarEstoqueDto)).ReturnsAsync(estoqueResponseDto);

            // Act
            var resultado = await _controller.Atualizar(estoqueId, atualizarEstoqueDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsType<EstoqueResponse>(okResult.Value);
        }

        [Fact]
        public async Task Remover_QuandoEstoqueExistir_DeveRetornarNoContent()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            _estoqueServicoMock.Setup(s => s.DeletarAsync(estoqueId)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.Remover(estoqueId);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeEstoques()
        {
            // Arrange
            var estoques = new List<EstoqueResponse> { new EstoqueResponse() };
            _estoqueServicoMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(estoques);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<EstoqueResponse>>(okResult.Value);
        }
    }
}
