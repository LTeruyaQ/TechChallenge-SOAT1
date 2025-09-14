using Core.DTOs.UseCases.Veiculo;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class VeiculoUseCasesUnitTests
{
    private readonly VeiculoUseCasesFixture _fixture;

    public VeiculoUseCasesUnitTests()
    {
        _fixture = new VeiculoUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarVeiculoCadastrado()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();

        _fixture.ConfigurarMockVeiculoGatewayParaCadastro(mockVeiculoGateway, veiculoEsperado);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway, mockUdt);

        // Act
        var resultado = await veiculoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.ClienteId.Should().Be(request.ClienteId);
        resultado.Marca.Should().Be(request.Marca);
        resultado.Modelo.Should().Be(request.Modelo);
        resultado.Ano.Should().Be(request.Ano);
        resultado.Placa.Should().Be(request.Placa);
        resultado.Cor.Should().Be(request.Cor);
        resultado.Anotacoes.Should().Be(request.Anotacoes);

        mockVeiculoGateway.Received(1).CadastrarAsync(Arg.Any<Veiculo>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComVeiculoSemAnotacoes_DeveCadastrarComSucesso()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoSemAnotacoes();
        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoSemAnotacoes();

        _fixture.ConfigurarMockVeiculoGatewayParaCadastro(mockVeiculoGateway, veiculoEsperado);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Anotacoes.Should().BeNull();
        resultado.Marca.Should().Be(request.Marca);
        resultado.Modelo.Should().Be(request.Modelo);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarVeiculoAtualizado()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var veiculoExistente = VeiculoUseCasesFixture.CriarVeiculoValido();
        var request = VeiculoUseCasesFixture.CriarAtualizarVeiculoUseCaseDtoValido();

        _fixture.ConfigurarMockVeiculoGatewayParaAtualizacao(mockVeiculoGateway, veiculoExistente);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway, mockUdt);

        // Act
        var resultado = await veiculoUseCases.AtualizarUseCaseAsync(veiculoExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(veiculoExistente.Id);
        resultado.ClienteId.Should().Be(request.ClienteId);
        resultado.Marca.Should().Be(request.Marca);
        resultado.Modelo.Should().Be(request.Modelo);
        resultado.Ano.Should().Be(request.Ano);
        resultado.Placa.Should().Be(request.Placa);
        resultado.Cor.Should().Be(request.Cor);
        resultado.Anotacoes.Should().Be(request.Anotacoes);

        mockVeiculoGateway.Received(1).ObterPorIdAsync(veiculoExistente.Id);
        mockVeiculoGateway.Received(1).EditarAsync(Arg.Any<Veiculo>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComAtualizacaoParcial_DeveAtualizarApenasPropriedadesInformadas()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var veiculoExistente = VeiculoUseCasesFixture.CriarVeiculoValido();
        var marcaOriginal = veiculoExistente.Marca;
        var corOriginal = veiculoExistente.Cor;
        var request = VeiculoUseCasesFixture.CriarAtualizarVeiculoParcial();

        _fixture.ConfigurarMockVeiculoGatewayParaAtualizacao(mockVeiculoGateway, veiculoExistente);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway, mockUdt);

        // Act
        var resultado = await veiculoUseCases.AtualizarUseCaseAsync(veiculoExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Modelo.Should().Be(request.Modelo); // Atualizado
        resultado.Placa.Should().Be(request.Placa); // Atualizado
        resultado.Anotacoes.Should().Be(request.Anotacoes); // Atualizado
        resultado.Marca.Should().Be(marcaOriginal); // Mantido (null no request)
        resultado.Cor.Should().Be(corOriginal); // Mantido (null no request)
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComVeiculoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var veiculoId = Guid.NewGuid();
        var request = VeiculoUseCasesFixture.CriarAtualizarVeiculoUseCaseDtoValido();

        _fixture.ConfigurarMockVeiculoGatewayParaVeiculoNaoEncontrado(mockVeiculoGateway, veiculoId);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act & Assert
        await veiculoUseCases
            .Invoking(x => x.AtualizarUseCaseAsync(veiculoId, request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Veículo não encontrado");

        mockVeiculoGateway.Received(1).ObterPorIdAsync(veiculoId);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdValido_DeveRetornarVeiculo()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var veiculoExistente = VeiculoUseCasesFixture.CriarVeiculoValido();

        mockVeiculoGateway.ObterPorIdAsync(veiculoExistente.Id).Returns(Task.FromResult(veiculoExistente));

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.ObterPorIdUseCaseAsync(veiculoExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(veiculoExistente.Id);
        resultado.Marca.Should().Be(veiculoExistente.Marca);
        resultado.Modelo.Should().Be(veiculoExistente.Modelo);
        resultado.Placa.Should().Be(veiculoExistente.Placa);

        mockVeiculoGateway.Received(1).ObterPorIdAsync(veiculoExistente.Id);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var veiculoId = Guid.NewGuid();

        _fixture.ConfigurarMockVeiculoGatewayParaVeiculoNaoEncontrado(mockVeiculoGateway, veiculoId);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act & Assert
        await veiculoUseCases
            .Invoking(x => x.ObterPorIdUseCaseAsync(veiculoId))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage($"Veículo com ID {veiculoId} não encontrado.");

        mockVeiculoGateway.Received(1).ObterPorIdAsync(veiculoId);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeVeiculos()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var veiculosEsperados = VeiculoUseCasesFixture.CriarListaVeiculosVariados();

        _fixture.ConfigurarMockVeiculoGatewayParaListagem(mockVeiculoGateway, veiculosEsperados);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(veiculosEsperados.Count);
        resultado.Should().BeEquivalentTo(veiculosEsperados);

        mockVeiculoGateway.Received(1).ObterTodosAsync();
    }

    [Fact]
    public async Task ObterPorClienteUseCaseAsync_ComClienteExistente_DeveRetornarVeiculosDoCliente()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var clienteId = Guid.NewGuid();
        var veiculosDoCliente = VeiculoUseCasesFixture.CriarListaVeiculosPorCliente(clienteId);

        _fixture.ConfigurarMockVeiculoGatewayParaBuscaPorCliente(mockVeiculoGateway, clienteId, veiculosDoCliente);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.ObterPorClienteUseCaseAsync(clienteId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(veiculosDoCliente.Count);
        resultado.Should().OnlyContain(v => v.ClienteId == clienteId);

        mockVeiculoGateway.Received(1).ObterVeiculoPorClienteAsync(clienteId);
    }

    [Fact]
    public async Task ObterPorPlacaUseCaseAsync_ComPlacaExistente_DeveRetornarVeiculo()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var veiculoExistente = VeiculoUseCasesFixture.CriarVeiculoValido();
        var veiculosComPlaca = new List<Veiculo> { veiculoExistente };

        _fixture.ConfigurarMockVeiculoGatewayParaBuscaPorPlaca(mockVeiculoGateway, veiculoExistente.Placa, veiculosComPlaca);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.ObterPorPlacaUseCaseAsync(veiculoExistente.Placa);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Placa.Should().Be(veiculoExistente.Placa);
        resultado.Id.Should().Be(veiculoExistente.Id);

        mockVeiculoGateway.Received(1).ObterVeiculoPorPlacaAsync(veiculoExistente.Placa);
    }

    [Fact]
    public async Task ObterPorPlacaUseCaseAsync_ComPlacaInexistente_DeveRetornarNull()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var placaInexistente = "XXX0000";
        var veiculosVazios = new List<Veiculo>();

        _fixture.ConfigurarMockVeiculoGatewayParaBuscaPorPlaca(mockVeiculoGateway, placaInexistente, veiculosVazios);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.ObterPorPlacaUseCaseAsync(placaInexistente);

        // Assert
        resultado.Should().BeNull();
        mockVeiculoGateway.Received(1).ObterVeiculoPorPlacaAsync(placaInexistente);
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComVeiculoExistente_DeveRetornarTrue()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var veiculoExistente = VeiculoUseCasesFixture.CriarVeiculoValido();

        _fixture.ConfigurarMockVeiculoGatewayParaDelecao(mockVeiculoGateway, veiculoExistente);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway, mockUdt);

        // Act
        var resultado = await veiculoUseCases.DeletarUseCaseAsync(veiculoExistente.Id);

        // Assert
        resultado.Should().BeTrue();

        mockVeiculoGateway.Received(1).ObterPorIdAsync(veiculoExistente.Id);
        mockVeiculoGateway.Received(1).DeletarAsync(Arg.Any<Veiculo>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComVeiculoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var veiculoId = Guid.NewGuid();

        _fixture.ConfigurarMockVeiculoGatewayParaVeiculoNaoEncontrado(mockVeiculoGateway, veiculoId);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act & Assert
        await veiculoUseCases
            .Invoking(x => x.DeletarUseCaseAsync(veiculoId))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Veículo não encontrado");

        mockVeiculoGateway.Received(1).ObterPorIdAsync(veiculoId);
        mockVeiculoGateway.Received(0).DeletarAsync(Arg.Any<Veiculo>());
    }

    [Theory]
    [InlineData(2020)]
    [InlineData(2015)]
    [InlineData(2023)]
    [InlineData(1990)]
    public async Task CadastrarUseCaseAsync_ComDiferentesAnos_DeveCadastrarCorretamente(int ano)
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        request.Ano = ano.ToString();

        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();
        veiculoEsperado.Ano = ano.ToString();

        _fixture.ConfigurarMockVeiculoGatewayParaCadastro(mockVeiculoGateway, veiculoEsperado);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Ano.Should().Be(ano.ToString());
    }

    [Theory]
    [InlineData("ABC1234")]
    [InlineData("XYZ9876")]
    [InlineData("BRA2E19")]
    [InlineData("OLD0000")]
    public async Task CadastrarUseCaseAsync_ComDiferentesPlacas_DeveCadastrarCorretamente(string placa)
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var request = VeiculoUseCasesFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        request.Placa = placa;

        var veiculoEsperado = VeiculoUseCasesFixture.CriarVeiculoValido();
        veiculoEsperado.Placa = placa;

        _fixture.ConfigurarMockVeiculoGatewayParaCadastro(mockVeiculoGateway, veiculoEsperado);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway);

        // Act
        var resultado = await veiculoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Placa.Should().Be(placa);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_DeveAtualizarDataAtualizacao()
    {
        // Arrange
        var mockVeiculoGateway = _fixture.CriarMockVeiculoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var veiculoExistente = VeiculoUseCasesFixture.CriarVeiculoValido();
        var dataAtualizacaoOriginal = veiculoExistente.DataAtualizacao;
        var request = VeiculoUseCasesFixture.CriarAtualizarVeiculoUseCaseDtoValido();

        _fixture.ConfigurarMockVeiculoGatewayParaAtualizacao(mockVeiculoGateway, veiculoExistente);

        var veiculoUseCases = _fixture.CriarVeiculoUseCases(mockVeiculoGateway, mockUdt);

        // Act
        var resultado = await veiculoUseCases.AtualizarUseCaseAsync(veiculoExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.DataAtualizacao.Should().Be(veiculoExistente.DataAtualizacao.Value);

    }

    [Fact]
    public void Constructor_ComParametrosNulos_DeveLancarArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            _fixture.CriarVeiculoUseCases(null));
    }
}
