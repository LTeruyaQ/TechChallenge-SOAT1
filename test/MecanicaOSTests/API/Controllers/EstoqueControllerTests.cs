using API.Controllers;
using API.DTOs.Request.Estoque;
using Aplicacao.UseCases.Estoque;
using Aplicacao.UseCases.Estoque.AtualizarEstoque;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using Aplicacao.UseCases.Estoque.DeletarEstoque;
using Aplicacao.UseCases.Estoque.ListaEstoque;
using Aplicacao.UseCases.Estoque.ObterEstoque;
using Dominio.Entidades;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MecanicaOSTests.API.Controllers
{
    public static class CriarEstoqueRequestMapper
    {
        public static Estoque ParaEntidade(CriarEstoqueRequest request) => new(request.Insumo, request.Descricao, request.Preco, request.QuantidadeDisponivel, request.QuantidadeMinima);
    }

    public static class EstoquePresenter
    {
        public static EstoqueResponse ParaResponse(Estoque estoque) => new() { Id = estoque.Id, Insumo = estoque.Insumo, Descricao = estoque.Descricao, Preco = estoque.Preco, QuantidadeDisponivel = estoque.QuantidadeDisponivel, QuantidadeMinima = estoque.QuantidadeMinima };
        public static IEnumerable<EstoqueResponse> ParaIEnumerableResponse(IEnumerable<Estoque> estoques) => estoques.Select(e => ParaResponse(e));
    }

    public class EstoqueControllerTests
    {
        private readonly Mock<ILogger<EstoqueController>> _mockLogger;
        private readonly Mock<ICriarEstoqueUseCase> _mockCriarEstoqueUseCase;
        private readonly Mock<IAtualizarEstoqueUseCase> _mockAtualizarEstoqueUseCase;
        private readonly Mock<IDeletarEstoqueUseCase> _mockDeletarEstoqueUseCase;
        private readonly Mock<IObterEstoquePorIdUseCase> _mockObterEstoquePorIdUseCase;
        private readonly Mock<IListarEstoqueUseCase> _mockListarEstoqueUseCase;
        private readonly EstoqueController _controller;

        public EstoqueControllerTests()
        {
            // ARRANGE: Configuração inicial dos mocks para todos os testes
            _mockLogger = new Mock<ILogger<EstoqueController>>();
            _mockCriarEstoqueUseCase = new Mock<ICriarEstoqueUseCase>();
            _mockAtualizarEstoqueUseCase = new Mock<IAtualizarEstoqueUseCase>();
            _mockDeletarEstoqueUseCase = new Mock<IDeletarEstoqueUseCase>();
            _mockObterEstoquePorIdUseCase = new Mock<IObterEstoquePorIdUseCase>();
            _mockListarEstoqueUseCase = new Mock<IListarEstoqueUseCase>();

            _controller = new EstoqueController(
                _mockLogger.Object,
                _mockCriarEstoqueUseCase.Object,
                _mockAtualizarEstoqueUseCase.Object,
                _mockDeletarEstoqueUseCase.Object,
                _mockObterEstoquePorIdUseCase.Object,
                _mockListarEstoqueUseCase.Object
            );
        }

        // --- Testes para a ação ObterTodos() ---

        [Fact]
        public async Task ObterTodos_QuandoSemErro_DeveRetornarOkComListaDeEstoques()
        {
            // Arrange
            var estoques = new List<Estoque> { new("Insumo1", "Desc1", 10m, 5, 2), new("Insumo2", "Desc2", 20m, 10, 3) };
            _mockListarEstoqueUseCase.Setup(uc => uc.ExecutarAsync()).ReturnsAsync(estoques);

            // Act
            var result = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEstoques = Assert.IsAssignableFrom<IEnumerable<EstoqueResponse>>(okResult.Value);
            Assert.Equal(2, returnedEstoques.Count());
        }

        [Fact]
        public async Task ObterTodos_QuandoExcecaoGenerica_DeveRetornar500()
        {
            // Arrange
            _mockListarEstoqueUseCase.Setup(uc => uc.ExecutarAsync()).ThrowsAsync(new Exception("Erro de teste"));

            // Act
            var result = await _controller.ObterTodos();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --- Testes para a ação ObterPorId(Guid id) ---

        [Fact]
        public async Task ObterPorId_QuandoEncontrado_DeveRetornarOkComEstoque()
        {
            // Arrange
            var estoque = new Estoque("InsumoTeste", "DescTeste", 10m, 5, 2);
            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(estoque.Id)).ReturnsAsync(estoque);

            // Act
            var result = await _controller.ObterPorId(estoque.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEstoque = Assert.IsType<EstoqueResponse>(okResult.Value);
            Assert.Equal(estoque.Id, returnedEstoque.Id);
        }

        [Fact]
        public async Task ObterPorId_QuandoNaoEncontrado_DeveRetornar404()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(id)).ThrowsAsync(new DadosNaoEncontradosException("Não encontrado"));

            // Act
            var result = await _controller.ObterPorId(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_QuandoExcecaoGenerica_DeveRetornar500()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(id)).ThrowsAsync(new Exception("Erro de teste"));

            // Act
            var result = await _controller.ObterPorId(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --- Testes para a ação Criar(CriarEstoqueRequest request) ---

        [Fact]
        public async Task Criar_QuandoValido_DeveRetornar201Created()
        {
            // Arrange
            var request = new CriarEstoqueRequest { Insumo = "InsumoNovo", Descricao = "DescNovo", Preco = 15m, QuantidadeDisponivel = 10, QuantidadeMinima = 5 };
            var estoqueEntidade = new Estoque("InsumoNovo", "DescNovo", 15m, 10, 5);
            _mockCriarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ReturnsAsync(estoqueEntidade);

            // Act
            var result = await _controller.Criar(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedResponse = Assert.IsType<EstoqueResponse>(createdResult.Value);
            Assert.NotNull(returnedResponse.Id);
        }

        [Fact]
        public async Task Criar_QuandoDominioInvalido_DeveRetornar400()
        {
            // Arrange
            var request = new CriarEstoqueRequest { Insumo = "", Descricao = "", Preco = 0, QuantidadeDisponivel = 0, QuantidadeMinima = 0 };
            _mockCriarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ThrowsAsync(new DomainException("Dados inválidos"));

            // Act
            var result = await _controller.Criar(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Criar_QuandoErroPersistencia_DeveRetornar500()
        {
            // Arrange
            var request = new CriarEstoqueRequest { Insumo = "Insumo", Descricao = "Desc", Preco = 1, QuantidadeDisponivel = 1, QuantidadeMinima = 1 };
            _mockCriarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ThrowsAsync(new PersistirDadosException("Erro"));

            // Act
            var result = await _controller.Criar(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Criar_QuandoExcecaoGenerica_DeveRetornar500()
        {
            // Arrange
            var request = new CriarEstoqueRequest { Insumo = "Insumo", Descricao = "Desc", Preco = 1, QuantidadeDisponivel = 1, QuantidadeMinima = 1 };
            _mockCriarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.Criar(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --- Testes para a ação Atualizar(Guid id, AtualizarEstoqueRequest request) ---

        [Fact]
        public async Task Atualizar_QuandoValido_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarEstoqueRequest { Descricao = "Nova Desc", Preco = 20m };
            var estoqueOriginal = new Estoque("Insumo", "Desc", 10m, 5, 2);
            var estoqueAtualizado = new Estoque("Insumo", "Nova Desc", 20m, 5, 2);

            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(id)).ReturnsAsync(estoqueOriginal);
            _mockAtualizarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ReturnsAsync(estoqueAtualizado);

            // Act
            var result = await _controller.Atualizar(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResponse = Assert.IsType<EstoqueResponse>(okResult.Value);
            Assert.Equal("Nova Desc", returnedResponse.Descricao);
        }

        [Fact]
        public async Task Atualizar_QuandoDominioInvalido_DeveRetornar400()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarEstoqueRequest { Insumo = "", Descricao = "" };
            var estoque = new Estoque("Insumo", "Desc", 10m, 5, 2);

            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(id)).ReturnsAsync(estoque);
            _mockAtualizarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ThrowsAsync(new DomainException("Dados inválidos"));

            // Act
            var result = await _controller.Atualizar(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Atualizar_QuandoEstoqueNaoEncontrado_DeveRetornar404()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarEstoqueRequest { Descricao = "Nova Desc", Preco = 20m };
            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(id)).ThrowsAsync(new DadosNaoEncontradosException("Estoque não encontrado"));

            // Act
            var result = await _controller.Atualizar(id, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Atualizar_QuandoErroPersistencia_DeveRetornar500()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarEstoqueRequest { Descricao = "Nova Desc", Preco = 20m };
            var estoque = new Estoque("Insumo", "Desc", 10m, 5, 2);
            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(id)).ReturnsAsync(estoque);
            _mockAtualizarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ThrowsAsync(new PersistirDadosException("Erro"));

            // Act
            var result = await _controller.Atualizar(id, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Atualizar_QuandoExcecaoGenerica_DeveRetornar500()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarEstoqueRequest { Descricao = "Nova Desc", Preco = 20m };
            var estoque = new Estoque("Insumo", "Desc", 10m, 5, 2);
            _mockObterEstoquePorIdUseCase.Setup(uc => uc.ExecutarAsync(id)).ReturnsAsync(estoque);
            _mockAtualizarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(It.IsAny<Estoque>())).ThrowsAsync(new Exception("Erro"));

            // Act
            var result = await _controller.Atualizar(id, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --- Testes para a ação Remover(Guid id) ---

        [Fact]
        public async Task Remover_QuandoExistente_DeveRetornar204NoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockDeletarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Remover(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Remover_QuandoNaoEncontrado_DeveRetornar404()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockDeletarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(id)).ThrowsAsync(new DadosNaoEncontradosException("Não encontrado"));

            // Act
            var result = await _controller.Remover(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Remover_QuandoExcecaoGenerica_DeveRetornar500()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockDeletarEstoqueUseCase.Setup(uc => uc.ExecutarAsync(id)).ThrowsAsync(new Exception("Erro de teste"));

            // Act
            var result = await _controller.Remover(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}