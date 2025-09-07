using Aplicacao.UseCases.Estoque.AtualizarEstoque;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using Aplicacao.UseCases.Estoque.DeletarEstoque;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Moq;
using Aplicacao.UseCases.Estoque.ObterEstoque;
using Aplicacao.UseCases.Estoque.ListaEstoque;
using Aplicacao.Interfaces.Gateways;

public class EstoqueUseCasesTests
{
    private readonly Mock<IEstoqueGateway> _mockGateway;
    private readonly Mock<IUnidadeDeTrabalho> _mockUdt;

    public EstoqueUseCasesTests()
    {
        _mockGateway = new Mock<IEstoqueGateway>();
        _mockUdt = new Mock<IUnidadeDeTrabalho>();
    }

    [Fact]
    public async Task CriarEstoque_QuandoSucesso_DeveChamarGatewayECorretamente()
    {
        // Arrange
        var estoque = new Estoque("Insumo", "Desc", 10m, 5, 2);
        _mockUdt.Setup(u => u.Commit()).ReturnsAsync(true);
        var useCase = new CriarEstoqueUseCase(_mockGateway.Object, _mockUdt.Object);

        // Act
        var resultado = await useCase.ExecutarAsync(estoque);

        // Assert
        _mockGateway.Verify(g => g.CadastrarAsync(estoque), Times.Once);
        _mockUdt.Verify(u => u.Commit(), Times.Once);
        Assert.Equal(estoque, resultado);
    }

    [Fact]
    public async Task CriarEstoque_QuandoErroNaPersistencia_DeveLancarExcecao()
    {
        // Arrange
        var estoque = new Estoque("Insumo", "Desc", 10m, 5, 2);
        _mockUdt.Setup(u => u.Commit()).ReturnsAsync(false);
        var useCase = new CriarEstoqueUseCase(_mockGateway.Object, _mockUdt.Object);

        // Act & Assert
        await Assert.ThrowsAsync<PersistirDadosException>(() => useCase.ExecutarAsync(estoque));
        _mockGateway.Verify(g => g.CadastrarAsync(estoque), Times.Once);
        _mockUdt.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task DeletarEstoque_QuandoSucesso_DeveRetornarTrue()
    {
        // Arrange
        var estoque = new Estoque("Insumo", "Desc", 10m, 5, 2);
        _mockGateway.Setup(g => g.ObterPorIdAsync(estoque.Id)).ReturnsAsync(estoque);
        _mockUdt.Setup(u => u.Commit()).ReturnsAsync(true);
        var useCase = new DeletarEstoqueUseCase(_mockGateway.Object, _mockUdt.Object);

        // Act
        var resultado = await useCase.ExecutarAsync(estoque.Id);

        // Assert
        _mockGateway.Verify(g => g.ObterPorIdAsync(estoque.Id), Times.Once);
        _mockGateway.Verify(g => g.DeletarAsync(estoque), Times.Once);
        _mockUdt.Verify(u => u.Commit(), Times.Once);
        Assert.True(resultado);
    }

    [Fact]
    public async Task DeletarEstoque_QuandoNaoEncontrado_DeveLancarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockGateway.Setup(g => g.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);
        var useCase = new DeletarEstoqueUseCase(_mockGateway.Object, _mockUdt.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => useCase.ExecutarAsync(id));
        _mockGateway.Verify(g => g.ObterPorIdAsync(id), Times.Once);
        _mockGateway.Verify(g => g.DeletarAsync(It.IsAny<Estoque>()), Times.Never);
        _mockUdt.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeletarEstoque_QuandoErroNaPersistencia_DeveRetornarFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var estoque = new Estoque("Insumo", "Desc", 10m, 5, 2);
        _mockGateway.Setup(g => g.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _mockUdt.Setup(u => u.Commit()).ReturnsAsync(false);
        var useCase = new DeletarEstoqueUseCase(_mockGateway.Object, _mockUdt.Object);

        // Act
        var resultado = await useCase.ExecutarAsync(id);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ListarEstoque_Sempre_DeveRetornarTodosOsEstoquesDoGateway()
    {
        // Arrange
        var estoques = new List<Estoque> { new("I1", "D1", 1m, 1, 1), new("I2", "D2", 2m, 2, 2) };
        _mockGateway.Setup(g => g.ObterTodosAsync()).ReturnsAsync(estoques);
        var useCase = new ListarEstoqueUseCase(_mockGateway.Object);

        // Act
        var resultado = await useCase.ExecutarAsync();

        // Assert
        Assert.Equal(estoques, resultado);
        _mockGateway.Verify(g => g.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterEstoquePorId_QuandoEncontrado_DeveRetornarEstoque()
    {
        // Arrange
        var estoque = new Estoque("I1", "D1", 1m, 1, 1);
        _mockGateway.Setup(g => g.ObterPorIdAsync(estoque.Id)).ReturnsAsync(estoque);
        var useCase = new ObterEstoquePorIdUseCase(_mockGateway.Object);

        // Act
        var resultado = await useCase.ExecutarAsync(estoque.Id);

        // Assert
        Assert.Equal(estoque, resultado);
        _mockGateway.Verify(g => g.ObterPorIdAsync(estoque.Id), Times.Once);
    }

    [Fact]
    public async Task ObterEstoquePorId_QuandoNaoEncontrado_DeveLancarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockGateway.Setup(g => g.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);
        var useCase = new ObterEstoquePorIdUseCase(_mockGateway.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => useCase.ExecutarAsync(id));
        _mockGateway.Verify(g => g.ObterPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task AtualizarEstoque_QuandoSucesso_DeveRetornarEstoqueAtualizado()
    {
        // Arrange
        var estoque = new Estoque("I1", "D1", 1m, 1, 1);
        _mockUdt.Setup(u => u.Commit()).ReturnsAsync(true);
        var useCase = new AtualizarEstoqueUseCase(_mockGateway.Object, _mockUdt.Object);

        // Act
        var resultado = await useCase.ExecutarAsync(estoque);

        // Assert
        _mockGateway.Verify(g => g.EditarAsync(estoque), Times.Once);
        _mockUdt.Verify(u => u.Commit(), Times.Once);
        Assert.Equal(estoque, resultado);
        Assert.NotEqual(default, resultado.DataAtualizacao); // Garante que a data foi atualizada
    }

    [Fact]
    public async Task AtualizarEstoque_QuandoErroNaPersistencia_DeveLancarExcecao()
    {
        // Arrange
        var estoque = new Estoque( "I1", "D1", 1m, 1, 1);
        _mockUdt.Setup(u => u.Commit()).ReturnsAsync(false);
        var useCase = new AtualizarEstoqueUseCase(_mockGateway.Object, _mockUdt.Object);

        // Act & Assert
        await Assert.ThrowsAsync<PersistirDadosException>(() => useCase.ExecutarAsync(estoque));
        _mockGateway.Verify(g => g.EditarAsync(estoque), Times.Once);
        _mockUdt.Verify(u => u.Commit(), Times.Once);
    }
}