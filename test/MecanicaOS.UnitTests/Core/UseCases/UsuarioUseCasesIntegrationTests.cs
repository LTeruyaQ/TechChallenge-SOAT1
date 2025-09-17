using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

/// <summary>
/// Testes de integração para UsuarioUseCases focando no comportamento da interface
/// após a migração para handlers individuais
/// </summary>
public class UsuarioUseCasesIntegrationTests
{
    private readonly UsuarioUseCasesFixture _fixture;

    public UsuarioUseCasesIntegrationTests()
    {
        _fixture = new UsuarioUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComUsuarioAdministrador_DeveRetornarUsuarioCadastrado()
    {
        // Arrange
        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioAdministradorDto();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();
        usuarioEsperado.Senha = null; // Senha não deve ser retornada

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.CadastrarUseCaseAsync(request).Returns(usuarioEsperado);

        // Act
        var resultado = await usuarioUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(usuarioEsperado.Email);
        resultado.TipoUsuario.Should().Be(TipoUsuario.Admin);
        resultado.Senha.Should().BeNull(); // Senha não deve ser retornada

        await usuarioUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComUsuarioCliente_DeveVincularClienteId()
    {
        // Arrange
        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioClienteDto();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioClienteValido();

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.CadastrarUseCaseAsync(request).Returns(usuarioEsperado);

        // Act
        var resultado = await usuarioUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TipoUsuario.Should().Be(TipoUsuario.Cliente);
        resultado.ClienteId.Should().NotBeNull();

        await usuarioUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComEmailJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioAdministradorDto();

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.CadastrarUseCaseAsync(request)
            .Returns<Usuario>(x => throw new DadosJaCadastradosException("Email já cadastrado"));

        // Act & Assert
        await usuarioUseCases.Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should().ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Email já cadastrado");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task CadastrarUseCaseAsync_ComDiferentesRecebeAlertaEstoque_DeveCadastrarCorretamente(bool? recebeAlerta)
    {
        // Arrange
        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioAdministradorDto();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();
        usuarioEsperado.RecebeAlertaEstoque = recebeAlerta ?? false;

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.CadastrarUseCaseAsync(request).Returns(usuarioEsperado);

        // Act
        var resultado = await usuarioUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.RecebeAlertaEstoque.Should().Be(recebeAlerta ?? false);

        await usuarioUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarUsuarioAtualizado()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var request = UsuarioUseCasesFixture.CriarAtualizarUsuarioUseCaseDtoValido();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();
        usuarioEsperado.Senha = null; // Senha não deve ser retornada

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.AtualizarUseCaseAsync(usuarioId, request).Returns(usuarioEsperado);

        // Act
        var resultado = await usuarioUseCases.AtualizarUseCaseAsync(usuarioId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(usuarioEsperado);
        resultado.Senha.Should().BeNull(); // Senha não deve ser retornada

        await usuarioUseCases.Received(1).AtualizarUseCaseAsync(usuarioId, request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComUsuarioInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var request = UsuarioUseCasesFixture.CriarAtualizarUsuarioUseCaseDtoValido();

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.AtualizarUseCaseAsync(usuarioId, request)
            .Returns<Usuario>(x => throw new DadosNaoEncontradosException("Usuário não encontrado"));

        // Act & Assert
        await usuarioUseCases.Invoking(x => x.AtualizarUseCaseAsync(usuarioId, request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Usuário não encontrado");
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_SemNovaSenha_DeveManterSenhaExistente()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var request = UsuarioUseCasesFixture.CriarAtualizarUsuarioUseCaseDtoValido();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.AtualizarUseCaseAsync(usuarioId, request).Returns(usuarioEsperado);

        // Act
        var resultado = await usuarioUseCases.AtualizarUseCaseAsync(usuarioId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(usuarioEsperado);

        await usuarioUseCases.Received(1).AtualizarUseCaseAsync(usuarioId, request);
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdValido_DeveRetornarUsuarioSemSenha()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();
        usuarioEsperado.Senha = null; // Senha não deve ser retornada

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.ObterPorIdUseCaseAsync(usuarioId).Returns(usuarioEsperado);

        // Act
        var resultado = await usuarioUseCases.ObterPorIdUseCaseAsync(usuarioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(usuarioEsperado);
        resultado.Senha.Should().BeNull();

        await usuarioUseCases.Received(1).ObterPorIdUseCaseAsync(usuarioId);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeUsuariosSemSenhas()
    {
        // Arrange
        var usuariosEsperados = new List<Usuario>
        {
            UsuarioUseCasesFixture.CriarUsuarioAdministradorValido(),
            UsuarioUseCasesFixture.CriarUsuarioClienteValido()
        };

        // Garantir que as senhas não sejam retornadas
        foreach (var usuario in usuariosEsperados)
        {
            usuario.Senha = null;
        }

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.ObterTodosUseCaseAsync().Returns(usuariosEsperados);

        // Act
        var resultado = await usuarioUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(u => u.Senha == null);
        resultado.Should().BeEquivalentTo(usuariosEsperados);

        await usuarioUseCases.Received(1).ObterTodosUseCaseAsync();
    }

    [Fact]
    public async Task ObterPorEmailUseCaseAsync_ComEmailExistente_DeveRetornarUsuario()
    {
        // Arrange
        var email = "admin@test.com";
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();
        usuarioEsperado.Email = email;

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.ObterPorEmailUseCaseAsync(email).Returns(usuarioEsperado);

        // Act
        var resultado = await usuarioUseCases.ObterPorEmailUseCaseAsync(email);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(email);

        await usuarioUseCases.Received(1).ObterPorEmailUseCaseAsync(email);
    }

    [Fact]
    public async Task ObterPorEmailUseCaseAsync_ComEmailInexistente_DeveRetornarNull()
    {
        // Arrange
        var email = "inexistente@test.com";

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.ObterPorEmailUseCaseAsync(email).Returns((Usuario?)null);

        // Act
        var resultado = await usuarioUseCases.ObterPorEmailUseCaseAsync(email);

        // Assert
        resultado.Should().BeNull();

        await usuarioUseCases.Received(1).ObterPorEmailUseCaseAsync(email);
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComUsuarioExistente_DeveRetornarTrue()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.DeletarUseCaseAsync(usuarioId).Returns(true);

        // Act
        var resultado = await usuarioUseCases.DeletarUseCaseAsync(usuarioId);

        // Assert
        resultado.Should().BeTrue();

        await usuarioUseCases.Received(1).DeletarUseCaseAsync(usuarioId);
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComUsuarioInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.DeletarUseCaseAsync(usuarioId)
            .Returns<bool>(x => throw new DadosNaoEncontradosException("Usuário não encontrado"));

        // Act & Assert
        await usuarioUseCases.Invoking(x => x.DeletarUseCaseAsync(usuarioId))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Usuário não encontrado");
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComUsuarioClienteSemDocumento_DeveLancarDadosInvalidosException()
    {
        // Arrange
        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioClienteDto();

        var usuarioUseCases = _fixture.CriarUsuarioUseCases();
        usuarioUseCases.CadastrarUseCaseAsync(request)
            .Returns<Usuario>(x => throw new DadosInvalidosException("Cliente deve ter documento informado"));

        // Act & Assert
        await usuarioUseCases.Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should().ThrowAsync<DadosInvalidosException>()
            .WithMessage("Cliente deve ter documento informado");
    }
}
