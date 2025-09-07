using API.Controllers;
using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using MecanicaOSTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOSTests.API.Controllers
{
    public static class EstoqueFixture
    {
        public static CriarEstoqueRequest CriarCadastrarEstoqueRequestValido()
        {
            return new CriarEstoqueRequest
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
        private readonly Mock<ILogger<EstoqueController>> _iLoggerServicoMock;
        private readonly Mock<ICriarEstoqueUseCase> _criarEstoqueUseCaseMock;
        private readonly EstoqueController _controller;

        public EstoqueControllerTests()
        {
            _estoqueServicoMock = new Mock<IEstoqueServico>();
            _iLoggerServicoMock = new Mock<ILogger<EstoqueController>>();
            _criarEstoqueUseCaseMock = new Mock<ICriarEstoqueUseCase>();
            _controller = new EstoqueController(_estoqueServicoMock.Object, _iLoggerServicoMock.Object);
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
            var estoqueResponseDto = new CriarEstoqueResponse { Id = Guid.NewGuid() };
            _criarEstoqueUseCaseMock.Setup(s => s.ExecuteAsync(cadastrarEstoqueDto)).ReturnsAsync(estoqueResponseDto);

            // Act
            var resultado = await _controller.Criar(cadastrarEstoqueDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPorId), createdAtActionResult.ActionName);
            Assert.Equal(estoqueResponseDto.Id, ((CriarEstoqueResponse)createdAtActionResult.Value).Id);
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
