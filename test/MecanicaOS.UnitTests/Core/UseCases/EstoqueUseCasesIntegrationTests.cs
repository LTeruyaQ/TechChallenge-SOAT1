using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

/// <summary>
/// Testes de integração para EstoqueUseCases focando no comportamento da interface
/// após a migração para handlers individuais
/// </summary>
public class EstoqueUseCasesIntegrationTests
{
    private readonly EstoqueUseCasesFixture _fixture;

    public EstoqueUseCasesIntegrationTests()
    {
        _fixture = new EstoqueUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarEstoqueCadastrado()
    {
        // Arrange
        var request = EstoqueUseCasesFixture.CriarCadastrarEstoqueUseCaseDtoValido();
        var estoqueEsperado = EstoqueUseCasesFixture.CriarEstoqueValido();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.CadastrarUseCaseAsync(request).Returns(estoqueEsperado);

        // Act
        var resultado = await estoqueUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Insumo.Should().Be(estoqueEsperado.Insumo);
        resultado.Descricao.Should().Be(estoqueEsperado.Descricao);
        resultado.QuantidadeDisponivel.Should().Be(estoqueEsperado.QuantidadeDisponivel);
        resultado.Preco.Should().Be(estoqueEsperado.Preco);

        await estoqueUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComNomeJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var request = EstoqueUseCasesFixture.CriarCadastrarEstoqueUseCaseDtoValido();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.CadastrarUseCaseAsync(request)
            .Returns<Estoque>(x => throw new DadosJaCadastradosException("Produto já cadastrado"));

        // Act & Assert
        await estoqueUseCases.Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should().ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Produto já cadastrado");
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarEstoqueAtualizado()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();
        var request = EstoqueUseCasesFixture.CriarAtualizarEstoqueUseCaseDtoValido();
        var estoqueEsperado = EstoqueUseCasesFixture.CriarEstoqueValido();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.AtualizarUseCaseAsync(estoqueId, request).Returns(estoqueEsperado);

        // Act
        var resultado = await estoqueUseCases.AtualizarUseCaseAsync(estoqueId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(estoqueEsperado);

        await estoqueUseCases.Received(1).AtualizarUseCaseAsync(estoqueId, request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();
        var request = EstoqueUseCasesFixture.CriarAtualizarEstoqueUseCaseDtoValido();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.AtualizarUseCaseAsync(estoqueId, request)
            .Returns<Estoque>(x => throw new DadosNaoEncontradosException("Estoque não encontrado"));

        // Act & Assert
        await estoqueUseCases.Invoking(x => x.AtualizarUseCaseAsync(estoqueId, request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Estoque não encontrado");
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdExistente_DeveRetornarEstoque()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();
        var estoqueEsperado = EstoqueUseCasesFixture.CriarEstoqueValido();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.ObterPorIdUseCaseAsync(estoqueId).Returns(estoqueEsperado);

        // Act
        var resultado = await estoqueUseCases.ObterPorIdUseCaseAsync(estoqueId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(estoqueEsperado);

        await estoqueUseCases.Received(1).ObterPorIdUseCaseAsync(estoqueId);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.ObterPorIdUseCaseAsync(estoqueId)
            .Returns<Estoque>(x => throw new DadosNaoEncontradosException("Estoque não encontrado"));

        // Act & Assert
        await estoqueUseCases.Invoking(x => x.ObterPorIdUseCaseAsync(estoqueId))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Estoque não encontrado");
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeEstoques()
    {
        // Arrange
        var estoquesEsperados = EstoqueUseCasesFixture.CriarListaEstoquesVariados();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.ObterTodosUseCaseAsync().Returns(estoquesEsperados);

        // Act
        var resultado = await estoqueUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(estoquesEsperados.Count);
        resultado.Should().BeEquivalentTo(estoquesEsperados);

        await estoqueUseCases.Received(1).ObterTodosUseCaseAsync();
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComIdExistente_DeveRetornarTrue()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.DeletarUseCaseAsync(estoqueId).Returns(true);

        // Act
        var resultado = await estoqueUseCases.DeletarUseCaseAsync(estoqueId);

        // Assert
        resultado.Should().BeTrue();

        await estoqueUseCases.Received(1).DeletarUseCaseAsync(estoqueId);
    }

    [Fact]
    public async Task ObterEstoqueCriticoUseCaseAsync_DeveRetornarEstoquesCriticos()
    {
        // Arrange
        var estoquesCriticos = new List<Estoque>
        {
            EstoqueUseCasesFixture.CriarEstoqueCritico(),
            EstoqueUseCasesFixture.CriarEstoqueZerado()
        };

        var estoqueUseCases = _fixture.CriarEstoqueUseCases();
        estoqueUseCases.ObterEstoqueCriticoUseCaseAsync().Returns(estoquesCriticos);

        // Act
        var resultado = await estoqueUseCases.ObterEstoqueCriticoUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(estoquesCriticos);

        await estoqueUseCases.Received(1).ObterEstoqueCriticoUseCaseAsync();
    }
}
