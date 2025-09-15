using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using NSubstitute;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class ClienteUseCasesUnitTests
{
    private readonly ClienteUseCasesFixture _fixture;

    public ClienteUseCasesUnitTests()
    {
        _fixture = new ClienteUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarClienteCadastrado()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockEnderecoGateway = _fixture.CriarMockEnderecoGateway();
        var mockContatoGateway = _fixture.CriarMockContatoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var request = ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoValido();
        var clienteEsperado = ClienteUseCasesFixture.CriarClienteValido();

        _fixture.ConfigurarMockClienteGatewayParaCadastro(mockClienteGateway, clienteEsperado);
        _fixture.ConfigurarMockEnderecoGateway(mockEnderecoGateway);
        _fixture.ConfigurarMockContatoGateway(mockContatoGateway);

        var clienteUseCases = _fixture.CriarClienteUseCases(
            mockClienteGateway, mockEnderecoGateway, mockContatoGateway, null, mockUdt);

        // Act
        var resultado = await clienteUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(request.Nome);
        resultado.Documento.Should().Be(request.Documento);
        resultado.TipoCliente.Should().Be(request.TipoCliente);
        resultado.Ativo.Should().BeTrue();

        mockClienteGateway.Received(1).ObterClientePorDocumentoAsync(request.Documento);
        mockEnderecoGateway.Received(1).CadastrarAsync(Arg.Any<Endereco>());
        mockContatoGateway.Received(1).CadastrarAsync(Arg.Any<Contato>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDocumentoJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var request = ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoValido();

        _fixture.ConfigurarMockClienteGatewayParaDocumentoJaCadastrado(mockClienteGateway, request.Documento);

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act & Assert
        await clienteUseCases
            .Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Cliente já cadastrado");

        mockClienteGateway.Received(1).ObterClientePorDocumentoAsync(request.Documento);
        mockClienteGateway.DidNotReceive().CadastrarAsync(Arg.Any<Cliente>());
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComPessoaJuridica_DevePermitirDataNascimentoNula()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockEnderecoGateway = _fixture.CriarMockEnderecoGateway();
        var mockContatoGateway = _fixture.CriarMockContatoGateway();

        var request = ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoPessoaJuridica();
        var clienteEsperado = ClienteUseCasesFixture.CriarClientePessoaJuridica();

        _fixture.ConfigurarMockClienteGatewayParaCadastro(mockClienteGateway, clienteEsperado);
        _fixture.ConfigurarMockEnderecoGateway(mockEnderecoGateway);
        _fixture.ConfigurarMockContatoGateway(mockContatoGateway);

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TipoCliente.Should().Be(TipoCliente.PessoaJuridico);
        resultado.DataNascimento.Should().BeNull();
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarClienteAtualizado()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockEnderecoGateway = _fixture.CriarMockEnderecoGateway();
        var mockContatoGateway = _fixture.CriarMockContatoGateway();

        var clienteExistente = ClienteUseCasesFixture.CriarClienteValido();
        var request = ClienteUseCasesFixture.CriarAtualizarClienteUseCaseDtoValido();
        request.Id = clienteExistente.Id; // Garantir que o Id está definido

        _fixture.ConfigurarMockClienteGatewayParaAtualizacao(mockClienteGateway, clienteExistente);
        _fixture.ConfigurarMockEnderecoGateway(mockEnderecoGateway);
        _fixture.ConfigurarMockContatoGateway(mockContatoGateway);

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.AtualizarUseCaseAsync(clienteExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(clienteExistente.Id);
        resultado.Nome.Should().Be(request.Nome);

        mockClienteGateway.Received(1).ObterPorIdAsync(clienteExistente.Id);
        mockClienteGateway.Received(1).EditarAsync(Arg.Any<Cliente>());
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var clienteId = Guid.NewGuid();
        var request = ClienteUseCasesFixture.CriarAtualizarClienteUseCaseDtoValido();

        _fixture.ConfigurarMockClienteGatewayParaClienteNaoEncontrado(mockClienteGateway, clienteId);

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act & Assert
        await clienteUseCases
            .Invoking(x => x.AtualizarUseCaseAsync(clienteId, request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("cliente não encontrado");

        await mockClienteGateway.Received(1).ObterPorIdAsync(clienteId);
        await mockClienteGateway.DidNotReceive().EditarAsync(Arg.Any<Cliente>());
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdValido_DeveRetornarCliente()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var clienteExistente = ClienteUseCasesFixture.CriarClienteValido();

        mockClienteGateway.ObterPorIdAsync(clienteExistente.Id).Returns(Task.FromResult(clienteExistente));

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.ObterPorIdUseCaseAsync(clienteExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(clienteExistente.Id);
        resultado.Nome.Should().Be(clienteExistente.Nome);
        resultado.Documento.Should().Be(clienteExistente.Documento);

        mockClienteGateway.Received(1).ObterPorIdAsync(clienteExistente.Id);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var clienteId = Guid.NewGuid();

        mockClienteGateway.ObterPorIdAsync(clienteId).Returns(Task.FromResult((Cliente?)null));
        mockClienteGateway.ObterClienteComVeiculoPorIdAsync(clienteId).Returns(Task.FromResult((Cliente?)null));

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.ObterPorIdUseCaseAsync(clienteId);

        // Assert
        resultado.Should().BeNull();
        await mockClienteGateway.Received(1).ObterPorIdAsync(clienteId);
        await mockClienteGateway.Received(1).ObterClienteComVeiculoPorIdAsync(clienteId);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdInexistente_DeveChamarObterClienteComVeiculoPorIdAsync()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var clienteId = Guid.NewGuid();

        mockClienteGateway.ObterPorIdAsync(clienteId).Returns(Task.FromResult((Cliente?)null));
        mockClienteGateway.ObterClienteComVeiculoPorIdAsync(clienteId).Returns(Task.FromResult((Cliente?)null));

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.ObterPorIdUseCaseAsync(clienteId);

        // Assert
        resultado.Should().BeNull();
        await mockClienteGateway.Received(1).ObterPorIdAsync(clienteId);
        await mockClienteGateway.Received(1).ObterClienteComVeiculoPorIdAsync(clienteId);
    }

    [Fact]
    public async Task ObterPorDocumentoUseCaseAsync_ComDocumentoValido_DeveRetornarCliente()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var clienteExistente = ClienteUseCasesFixture.CriarClienteValido();

        mockClienteGateway.ObterClientePorDocumentoAsync(clienteExistente.Documento).Returns(Task.FromResult(clienteExistente));

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.ObterPorDocumentoUseCaseAsync(clienteExistente.Documento);

        resultado.Nome.Should().Be(clienteExistente.Nome);

        mockClienteGateway.Received(1).ObterClientePorDocumentoAsync(clienteExistente.Documento);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeClientes()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var clientesEsperados = new List<Cliente>
        {
            ClienteUseCasesFixture.CriarClienteValido(),
            ClienteUseCasesFixture.CriarClientePessoaJuridica()
        };

        mockClienteGateway.ObterTodosClienteComVeiculoAsync().Returns(Task.FromResult((IEnumerable<Cliente>)clientesEsperados));

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(clientesEsperados);

        mockClienteGateway.Received(1).ObterTodosClienteComVeiculoAsync();
    }

    [Fact]
    public async Task RemoverUseCaseAsync_ComClienteExistente_DeveRemoverComSucesso()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockEnderecoGateway = _fixture.CriarMockEnderecoGateway();
        var mockContatoGateway = _fixture.CriarMockContatoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var clienteExistente = ClienteUseCasesFixture.CriarClienteValido();

        mockClienteGateway.ObterPorIdAsync(clienteExistente.Id).Returns(Task.FromResult(clienteExistente));
        mockClienteGateway.DeletarAsync(Arg.Any<Cliente>()).Returns(Task.FromResult(true));
        mockUdt.Commit().Returns(Task.FromResult(true));

        var clienteUseCases = _fixture.CriarClienteUseCases(
            mockClienteGateway, mockEnderecoGateway, mockContatoGateway, null, mockUdt);

        // Act
        await clienteUseCases.RemoverUseCaseAsync(clienteExistente.Id);

        // Assert
        mockClienteGateway.Received(1).ObterPorIdAsync(clienteExistente.Id);
        mockClienteGateway.Received(1).DeletarAsync(Arg.Any<Cliente>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task RemoverUseCaseAsync_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var clienteId = Guid.NewGuid();

        _fixture.ConfigurarMockClienteGatewayParaClienteNaoEncontrado(mockClienteGateway, clienteId);

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act & Assert
        await clienteUseCases
            .Invoking(x => x.RemoverUseCaseAsync(clienteId))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Cliente não encontrado");

        mockClienteGateway.Received(1).ObterPorIdAsync(clienteId);
        mockClienteGateway.DidNotReceive().DeletarAsync(Arg.Any<Cliente>());
    }

    [Theory]
    [InlineData(TipoCliente.PessoaFisica)]
    [InlineData(TipoCliente.PessoaJuridico)]
    public async Task CadastrarUseCaseAsync_ComDiferentesTiposCliente_DeveCadastrarCorretamente(TipoCliente tipoCliente)
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockEnderecoGateway = _fixture.CriarMockEnderecoGateway();
        var mockContatoGateway = _fixture.CriarMockContatoGateway();

        var request = tipoCliente == TipoCliente.PessoaFisica
            ? ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoValido()
            : ClienteUseCasesFixture.CriarCadastrarClienteUseCaseDtoPessoaJuridica();

        _fixture.ConfigurarMockClienteGatewayParaCadastro(mockClienteGateway);
        _fixture.ConfigurarMockEnderecoGateway(mockEnderecoGateway);
        _fixture.ConfigurarMockContatoGateway(mockContatoGateway);

        var clienteUseCases = _fixture.CriarClienteUseCases(mockClienteGateway);

        // Act
        var resultado = await clienteUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TipoCliente.Should().Be(tipoCliente);

        if (tipoCliente == TipoCliente.PessoaJuridico)
        {
            resultado.DataNascimento.Should().BeNull();
            resultado.Sexo.Should().BeNull();
        }
        else
        {
            resultado.DataNascimento.Should().NotBeNull();
            resultado.Sexo.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void Constructor_ComParametrosNulos_DeveLancarArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            _fixture.CriarClienteUseCases(null));
        
        Assert.Throws<ArgumentNullException>(() => 
            _fixture.CriarClienteUseCases(
                _fixture.CriarMockClienteGateway(), 
                null));
        
        Assert.Throws<ArgumentNullException>(() => 
            _fixture.CriarClienteUseCases(
                _fixture.CriarMockClienteGateway(), 
                _fixture.CriarMockEnderecoGateway(), 
                null));
    }
}
