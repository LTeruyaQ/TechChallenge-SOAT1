using Core.DTOs.UseCases.Veiculo;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.UseCases;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

/// <summary>
/// Testes de integração para VeiculoUseCases focando no comportamento da interface
/// após a migração para handlers individuais
/// </summary>
public class VeiculoUseCasesIntegrationTests
{
    private readonly VeiculoUseCasesFixture _fixture;

    public VeiculoUseCasesIntegrationTests()
    {
        _fixture = new VeiculoUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarVeiculoCadastrado()
    {
        // Arrange
        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.CadastrarUseCaseAsync(request).Returns(veiculoEsperado);

        // Act
        var resultado = await veiculoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Marca.Should().Be(veiculoEsperado.Marca);
        resultado.Modelo.Should().Be(veiculoEsperado.Modelo);
        resultado.Placa.Should().Be(veiculoEsperado.Placa);
        resultado.ClienteId.Should().Be(veiculoEsperado.ClienteId);

        await veiculoUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComPlacaJaCadastrada_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoUseCaseDtoValido();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.CadastrarUseCaseAsync(request)
            .Returns<Veiculo>(x => throw new DadosJaCadastradosException("Placa já cadastrada"));

        // Act & Assert
        await veiculoUseCases.Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should().ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Placa já cadastrada");
    }

    [Theory]
    [InlineData("ABC1234")]
    [InlineData("XYZ9876")]
    [InlineData("BRA2E19")]
    [InlineData("OLD0000")]
    public async Task CadastrarUseCaseAsync_ComDiferentesPlacas_DeveCadastrarCorretamente(string placa)
    {
        // Arrange
        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();
        veiculoEsperado.Placa = placa;

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.CadastrarUseCaseAsync(request).Returns(veiculoEsperado);

        // Act
        var resultado = await veiculoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Placa.Should().Be(placa);

        await veiculoUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarVeiculoAtualizado()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();
        var request = VeiculoUseCasesFixture.CriarAtualizarVeiculoUseCaseDtoValido();
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.AtualizarUseCaseAsync(veiculoId, request).Returns(veiculoEsperado);

        // Act
        var resultado = await veiculoUseCases.AtualizarUseCaseAsync(veiculoId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(veiculoEsperado);

        await veiculoUseCases.Received(1).AtualizarUseCaseAsync(veiculoId, request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComVeiculoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();
        var request = VeiculoUseCasesFixture.CriarAtualizarVeiculoUseCaseDtoValido();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.AtualizarUseCaseAsync(veiculoId, request)
            .Returns<Veiculo>(x => throw new DadosNaoEncontradosException("Veículo não encontrado"));

        // Act & Assert
        await veiculoUseCases.Invoking(x => x.AtualizarUseCaseAsync(veiculoId, request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Veículo não encontrado");
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdExistente_DeveRetornarVeiculo()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.ObterPorIdUseCaseAsync(veiculoId).Returns(veiculoEsperado);

        // Act
        var resultado = await veiculoUseCases.ObterPorIdUseCaseAsync(veiculoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(veiculoEsperado);

        await veiculoUseCases.Received(1).ObterPorIdUseCaseAsync(veiculoId);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.ObterPorIdUseCaseAsync(veiculoId)
            .Returns<Veiculo>(x => throw new DadosNaoEncontradosException("Veículo não encontrado"));

        // Act & Assert
        await veiculoUseCases.Invoking(x => x.ObterPorIdUseCaseAsync(veiculoId))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Veículo não encontrado");
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeVeiculos()
    {
        // Arrange
        var veiculosEsperados = VeiculoUseCasesFixture.CriarListaVeiculosVariados();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.ObterTodosUseCaseAsync().Returns(veiculosEsperados);

        // Act
        var resultado = await veiculoUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(veiculosEsperados.Count);
        resultado.Should().BeEquivalentTo(veiculosEsperados);

        await veiculoUseCases.Received(1).ObterTodosUseCaseAsync();
    }

    [Fact]
    public async Task ObterPorClienteUseCaseAsync_ComClienteExistente_DeveRetornarVeiculosDoCliente()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var veiculosEsperados = new List<Veiculo>
        {
            VeiculoUseCasesFixture.CriarVeiculoValido(),
            VeiculoUseCasesFixture.CriarVeiculoValido()
        };
        
        // Configurar todos os veículos para terem o mesmo clienteId
        foreach (var veiculo in veiculosEsperados)
        {
            veiculo.ClienteId = clienteId;
        }

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.ObterPorClienteUseCaseAsync(clienteId).Returns(veiculosEsperados);

        // Act
        var resultado = await veiculoUseCases.ObterPorClienteUseCaseAsync(clienteId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().OnlyContain(v => v.ClienteId == clienteId);
        resultado.Should().BeEquivalentTo(veiculosEsperados);

        await veiculoUseCases.Received(1).ObterPorClienteUseCaseAsync(clienteId);
    }

    [Fact]
    public async Task ObterPorPlacaUseCaseAsync_ComPlacaExistente_DeveRetornarVeiculo()
    {
        // Arrange
        var placa = "ABC1234";
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();
        veiculoEsperado.Placa = placa;

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.ObterPorPlacaUseCaseAsync(placa).Returns(veiculoEsperado);

        // Act
        var resultado = await veiculoUseCases.ObterPorPlacaUseCaseAsync(placa);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Placa.Should().Be(placa);

        await veiculoUseCases.Received(1).ObterPorPlacaUseCaseAsync(placa);
    }

    [Fact]
    public async Task ObterPorPlacaUseCaseAsync_ComPlacaInexistente_DeveRetornarNull()
    {
        // Arrange
        var placa = "XYZ9999";

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.ObterPorPlacaUseCaseAsync(placa).Returns((Veiculo?)null);

        // Act
        var resultado = await veiculoUseCases.ObterPorPlacaUseCaseAsync(placa);

        // Assert
        resultado.Should().BeNull();

        await veiculoUseCases.Received(1).ObterPorPlacaUseCaseAsync(placa);
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComVeiculoExistente_DeveRetornarTrue()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.DeletarUseCaseAsync(veiculoId).Returns(true);

        // Act
        var resultado = await veiculoUseCases.DeletarUseCaseAsync(veiculoId);

        // Assert
        resultado.Should().BeTrue();

        await veiculoUseCases.Received(1).DeletarUseCaseAsync(veiculoId);
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComVeiculoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.DeletarUseCaseAsync(veiculoId)
            .Returns<bool>(x => throw new DadosNaoEncontradosException("Veículo não encontrado"));

        // Act & Assert
        await veiculoUseCases.Invoking(x => x.DeletarUseCaseAsync(veiculoId))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Veículo não encontrado");
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComVeiculoSemAnotacoes_DeveCadastrarComSucesso()
    {
        // Arrange
        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();
        veiculoEsperado.Anotacoes = null;

        var veiculoUseCases = _fixture.CriarVeiculoUseCases();
        veiculoUseCases.CadastrarUseCaseAsync(request).Returns(veiculoEsperado);

        // Act
        var resultado = await veiculoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Anotacoes.Should().BeNull();

        await veiculoUseCases.Received(1).CadastrarUseCaseAsync(request);
    }
}
