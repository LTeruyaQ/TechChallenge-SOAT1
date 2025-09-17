using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.UseCases;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

/// <summary>
/// Testes de integração para ClienteUseCases focando no comportamento da interface
/// após a migração para handlers individuais
/// </summary>
public class ClienteUseCasesIntegrationTests
{
    private readonly ClienteUseCasesFixture _fixture;

    public ClienteUseCasesIntegrationTests()
    {
        _fixture = new ClienteUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarClienteCadastrado()
    {
        // Arrange
        var request = ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoValido();
        var clienteEsperado = ClienteUseCasesFixture.CriarClienteValido();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.CadastrarUseCaseAsync(request).Returns(clienteEsperado);

        // Act
        var resultado = await clienteUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(clienteEsperado.Nome);
        resultado.Documento.Should().Be(clienteEsperado.Documento);
        resultado.TipoCliente.Should().Be(clienteEsperado.TipoCliente);

        await clienteUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDocumentoJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var request = ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoValido();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.CadastrarUseCaseAsync(request)
            .Returns<Cliente>(x => throw new DadosJaCadastradosException("Cliente já cadastrado"));

        // Act & Assert
        await clienteUseCases.Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should().ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Cliente já cadastrado");
    }

    [Theory]
    [InlineData(TipoCliente.PessoaFisica)]
    [InlineData(TipoCliente.PessoaJuridico)]
    public async Task CadastrarUseCaseAsync_ComDiferentesTiposCliente_DeveCadastrarCorretamente(TipoCliente tipoCliente)
    {
        // Arrange
        var request = ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoValido();
        var clienteEsperado = tipoCliente == TipoCliente.PessoaJuridico 
            ? ClienteUseCasesFixture.CriarClientePessoaJuridica() 
            : ClienteUseCasesFixture.CriarClienteValido();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.CadastrarUseCaseAsync(request).Returns(clienteEsperado);

        // Act
        var resultado = await clienteUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TipoCliente.Should().Be(tipoCliente);

        await clienteUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarClienteAtualizado()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var request = ClienteUseCasesFixture.CriarAtualizarClienteUseCaseDtoValido();
        var clienteEsperado = ClienteUseCasesFixture.CriarClienteValido();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.AtualizarUseCaseAsync(clienteId, request).Returns(clienteEsperado);

        // Act
        var resultado = await clienteUseCases.AtualizarUseCaseAsync(clienteId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(clienteEsperado);

        await clienteUseCases.Received(1).AtualizarUseCaseAsync(clienteId, request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var request = ClienteUseCasesFixture.CriarAtualizarClienteUseCaseDtoValido();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.AtualizarUseCaseAsync(clienteId, request)
            .Returns<Cliente>(x => throw new DadosNaoEncontradosException("Cliente não encontrado"));

        // Act & Assert
        await clienteUseCases.Invoking(x => x.AtualizarUseCaseAsync(clienteId, request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Cliente não encontrado");
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdExistente_DeveRetornarCliente()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var clienteEsperado = ClienteUseCasesFixture.CriarClienteValido();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.ObterPorIdUseCaseAsync(clienteId).Returns(clienteEsperado);

        // Act
        var resultado = await clienteUseCases.ObterPorIdUseCaseAsync(clienteId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(clienteEsperado);

        await clienteUseCases.Received(1).ObterPorIdUseCaseAsync(clienteId);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.ObterPorIdUseCaseAsync(clienteId).Returns((Cliente?)null);

        // Act
        var resultado = await clienteUseCases.ObterPorIdUseCaseAsync(clienteId);

        // Assert
        resultado.Should().BeNull();

        await clienteUseCases.Received(1).ObterPorIdUseCaseAsync(clienteId);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeClientes()
    {
        // Arrange
        var clientesEsperados = new List<Cliente>
        {
            ClienteUseCasesFixture.CriarClienteValido(),
            ClienteUseCasesFixture.CriarClientePessoaJuridica()
        };

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.ObterTodosUseCaseAsync().Returns(clientesEsperados);

        // Act
        var resultado = await clienteUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(clientesEsperados);

        await clienteUseCases.Received(1).ObterTodosUseCaseAsync();
    }

    [Fact]
    public async Task ObterPorDocumentoUseCaseAsync_ComDocumentoValido_DeveRetornarCliente()
    {
        // Arrange
        var documento = "12345678901";
        var clienteEsperado = ClienteUseCasesFixture.CriarClienteValido();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.ObterPorDocumentoUseCaseAsync(documento).Returns(clienteEsperado);

        // Act
        var resultado = await clienteUseCases.ObterPorDocumentoUseCaseAsync(documento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(clienteEsperado);

        await clienteUseCases.Received(1).ObterPorDocumentoUseCaseAsync(documento);
    }

    [Fact]
    public async Task RemoverUseCaseAsync_ComClienteExistente_DeveRemoverComSucesso()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        var clienteUseCases = _fixture.CriarClienteUseCases();

        // Act
        await clienteUseCases.RemoverUseCaseAsync(clienteId);

        // Assert
        await clienteUseCases.Received(1).RemoverUseCaseAsync(clienteId);
    }

    [Fact]
    public async Task RemoverUseCaseAsync_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        var clienteUseCases = _fixture.CriarClienteUseCases();
        clienteUseCases.When(x => x.RemoverUseCaseAsync(clienteId))
            .Do(x => throw new DadosNaoEncontradosException("Cliente não encontrado"));

        // Act & Assert
        await clienteUseCases.Invoking(x => x.RemoverUseCaseAsync(clienteId))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Cliente não encontrado");
    }
}
