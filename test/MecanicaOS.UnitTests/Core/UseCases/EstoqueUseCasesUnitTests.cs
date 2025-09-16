using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class EstoqueUseCasesUnitTests
{
    private readonly EstoqueUseCasesFixture _fixture;

    public EstoqueUseCasesUnitTests()
    {
        _fixture = new EstoqueUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarEstoqueCadastrado()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var request = EstoqueUseCasesFixture.CriarCadastrarEstoqueUseCaseDtoValido();
        var estoqueEsperado = EstoqueUseCasesFixture.CriarEstoqueValido();

        _fixture.ConfigurarMockEstoqueGatewayParaCadastro(mockEstoqueGateway, estoqueEsperado);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(
            mockEstoqueGateway, null, mockUdt);

        // Act
        var resultado = await estoqueUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Insumo.Should().Be(request.Insumo);
        resultado.Descricao.Should().Be(request.Descricao);
        resultado.QuantidadeDisponivel.Should().Be(request.QuantidadeDisponivel);
        resultado.Preco.Should().Be(request.Preco);
        resultado.Ativo.Should().BeTrue();

        mockEstoqueGateway.Received(1).ObterTodosAsync();
        mockEstoqueGateway.Received(1).CadastrarAsync(Arg.Any<Estoque>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComNomeJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var request = EstoqueUseCasesFixture.CriarCadastrarEstoqueUseCaseDtoValido();

        _fixture.ConfigurarMockEstoqueGatewayParaNomeJaCadastrado(mockEstoqueGateway, request.Insumo);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act & Assert
        await estoqueUseCases
            .Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Produto já cadastrado");

        mockEstoqueGateway.Received(1).ObterTodosAsync();
        mockEstoqueGateway.Received(0).CadastrarAsync(Arg.Any<Estoque>());
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComQuantidadeZero_DeveCadastrarComSucesso()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var request = EstoqueUseCasesFixture.CriarCadastrarEstoqueUseCaseDtoComQuantidadeZero();
        var estoqueEsperado = EstoqueUseCasesFixture.CriarEstoqueZerado();

        _fixture.ConfigurarMockEstoqueGatewayParaCadastro(mockEstoqueGateway, estoqueEsperado);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act
        var resultado = await estoqueUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.QuantidadeDisponivel.Should().Be(0);
        resultado.Insumo.Should().Be(request.Insumo);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarEstoqueAtualizado()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var estoqueExistente = EstoqueUseCasesFixture.CriarEstoqueValido();
        var request = EstoqueUseCasesFixture.CriarAtualizarEstoqueUseCaseDtoValido();

        _fixture.ConfigurarMockEstoqueGatewayParaAtualizacao(mockEstoqueGateway, estoqueExistente);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(
            mockEstoqueGateway, null, mockUdt);

        // Act
        var resultado = await estoqueUseCases.AtualizarUseCaseAsync(estoqueExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(estoqueExistente.Id);
        resultado.Insumo.Should().Be(request.Insumo);
        resultado.Descricao.Should().Be(request.Descricao);
        resultado.QuantidadeDisponivel.Should().Be(request.QuantidadeDisponivel);

        mockEstoqueGateway.Received(1).ObterPorIdAsync(estoqueExistente.Id);
        mockEstoqueGateway.Received(1).EditarAsync(Arg.Any<Estoque>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var estoqueId = Guid.NewGuid();
        var request = EstoqueUseCasesFixture.CriarAtualizarEstoqueUseCaseDtoValido();

        _fixture.ConfigurarMockEstoqueGatewayParaEstoqueNaoEncontrado(mockEstoqueGateway, estoqueId);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act & Assert
        await estoqueUseCases
            .Invoking(x => x.AtualizarUseCaseAsync(estoqueId, request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Estoque não encontrado");

        mockEstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
        mockEstoqueGateway.DidNotReceive().EditarAsync(Arg.Any<Estoque>());
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdValido_DeveRetornarEstoque()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var estoqueExistente = EstoqueUseCasesFixture.CriarEstoqueValido();

        mockEstoqueGateway
            .ObterPorIdAsync(estoqueExistente.Id)
            .Returns(Task.FromResult(estoqueExistente));

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act
        var resultado = await estoqueUseCases.ObterPorIdUseCaseAsync(estoqueExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(estoqueExistente.Id);
        resultado.Insumo.Should().Be(estoqueExistente.Insumo);
        resultado.QuantidadeDisponivel.Should().Be(estoqueExistente.QuantidadeDisponivel);

        mockEstoqueGateway.Received(1).ObterPorIdAsync(estoqueExistente.Id);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeEstoques()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var estoquesEsperados = EstoqueUseCasesFixture.CriarListaEstoquesVariados();

        _fixture.ConfigurarMockEstoqueGatewayParaListagem(mockEstoqueGateway, estoquesEsperados);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act
        var resultado = await estoqueUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(estoquesEsperados.Count);
        resultado.Should().BeEquivalentTo(estoquesEsperados);

        mockEstoqueGateway.Received(1).ObterTodosAsync();
    }

    [Fact]
    public async Task ObterEstoqueCriticoUseCaseAsync_DeveRetornarApenasEstoquesCriticos()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var todosEstoques = EstoqueUseCasesFixture.CriarListaEstoquesVariados();
        var estoquesCriticos = todosEstoques.Where(e => e.QuantidadeDisponivel <= e.QuantidadeMinima).ToList();

        _fixture.ConfigurarMockEstoqueGatewayParaListagem(mockEstoqueGateway, todosEstoques);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act
        var resultado = await estoqueUseCases.ObterEstoqueCriticoUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(estoquesCriticos.Count);
        resultado.Should().OnlyContain(e => e.QuantidadeDisponivel <= e.QuantidadeMinima);

        mockEstoqueGateway.Received(1).ObterEstoqueCriticoAsync();
    }

    [Fact]
    public async Task ExcluirUseCaseAsync_ComEstoqueExistente_DeveExcluirComSucesso()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var estoqueId = Guid.NewGuid();

        _fixture.ConfigurarMockEstoqueGatewayParaEstoqueNaoEncontrado(mockEstoqueGateway, estoqueId);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act & Assert
        await estoqueUseCases
            .Invoking(x => x.DeletarUseCaseAsync(estoqueId))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Estoque não encontrado");

        mockEstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
        mockEstoqueGateway.DidNotReceive().DeletarAsync(Arg.Any<Estoque>());
    }

    [Theory]
    [InlineData(10, 5)]  // Quantidade normal
    [InlineData(5, 5)]   // Quantidade igual ao mínimo (crítico)
    [InlineData(3, 10)]  // Quantidade abaixo do mínimo (crítico)
    [InlineData(0, 5)]   // Quantidade zero (crítico)
    public async Task CadastrarUseCaseAsync_ComDiferentesQuantidades_DeveCadastrarCorretamente(
        int quantidadeDisponivel, int quantidadeMinima)
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var request = EstoqueUseCasesFixture.CriarCadastrarEstoqueUseCaseDtoValido();
        request.QuantidadeDisponivel = quantidadeDisponivel;
        request.QuantidadeMinima = quantidadeMinima;

        var estoqueEsperado = EstoqueUseCasesFixture.CriarEstoqueValido();
        estoqueEsperado.QuantidadeDisponivel = quantidadeDisponivel;
        estoqueEsperado.QuantidadeMinima = quantidadeMinima;

        _fixture.ConfigurarMockEstoqueGatewayParaCadastro(mockEstoqueGateway, estoqueEsperado);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act
        var resultado = await estoqueUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.QuantidadeDisponivel.Should().Be(quantidadeDisponivel);
        resultado.QuantidadeMinima.Should().Be(quantidadeMinima);
    }

    [Fact]
    public async Task AtualizarQuantidadeUseCaseAsync_ComQuantidadeValida_DeveAtualizarComSucesso()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var estoqueExistente = EstoqueUseCasesFixture.CriarEstoqueValido();
        var novaQuantidade = 75;

        _fixture.ConfigurarMockEstoqueGatewayParaMovimentacao(
            mockEstoqueGateway, estoqueExistente, novaQuantidade);

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(
            mockEstoqueGateway, null, mockUdt);

        // Act
        await estoqueUseCases.AtualizarUseCaseAsync(estoqueExistente.Id, new AtualizarEstoqueUseCaseDto { QuantidadeDisponivel = novaQuantidade });

        // Assert
        mockEstoqueGateway.Received(1).ObterPorIdAsync(estoqueExistente.Id);
        mockEstoqueGateway.Received(1).EditarAsync(Arg.Any<Estoque>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdExistente_DeveRetornarEstoque()
    {
        // Arrange
        var mockEstoqueGateway = _fixture.CriarMockEstoqueGateway();
        var estoqueExistente = EstoqueUseCasesFixture.CriarEstoqueValido();

        mockEstoqueGateway
            .ObterPorIdAsync(estoqueExistente.Id)
            .Returns(Task.FromResult(estoqueExistente));

        var estoqueUseCases = _fixture.CriarEstoqueUseCases(mockEstoqueGateway);

        // Act
        var resultado = await estoqueUseCases.ObterPorIdUseCaseAsync(estoqueExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Insumo.Should().Be(estoqueExistente.Insumo);
        resultado.Id.Should().Be(estoqueExistente.Id);

        mockEstoqueGateway.Received(1).ObterPorIdAsync(estoqueExistente.Id);
    }

    [Fact]
    public void Constructor_ComParametrosNulos_DeveLancarArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _fixture.CriarEstoqueUseCases(null));
    }
}
