using Core.DTOs.UseCases.Autenticacao;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.UseCases;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class AutenticacaoUseCasesIntegrationTests
{
    private readonly AutenticacaoUseCasesFixture _fixture;

    public AutenticacaoUseCasesIntegrationTests()
    {
        _fixture = new AutenticacaoUseCasesFixture();
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComCredenciaisValidas_DeveRetornarAutenticacaoDto()
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();
        
        var autenticacaoEsperada = new AutenticacaoDto
        {
            Token = "token_jwt_valido",
            Usuario = usuario,
            Permissoes = new List<string> { "administrador" }
        };

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request).Returns(autenticacaoEsperada);

        // Act
        var resultado = await mockAutenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().Be("token_jwt_valido");
        resultado.Usuario.Should().NotBeNull();
        resultado.Usuario.Email.Should().Be(usuario.Email);
        resultado.Permissoes.Should().NotBeEmpty();
        resultado.Permissoes.Should().Contain("administrador");

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComCredenciaisInvalidas_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoComEmailInvalido();

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request)
            .Returns(Task.FromException<AutenticacaoDto>(new CredenciaisInvalidasException("Credenciais inválidas")));

        // Act & Assert
        await mockAutenticacaoUseCases
            .Invoking(x => x.AutenticarUseCaseAsync(request))
            .Should()
            .ThrowAsync<CredenciaisInvalidasException>()
            .WithMessage("Credenciais inválidas");

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComUsuarioInativo_DeveLancarUsuarioInativoException()
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request)
            .Returns(Task.FromException<AutenticacaoDto>(new UsuarioInativoException("Usuário inativo. Entre em contato com o administrador.")));

        // Act & Assert
        await mockAutenticacaoUseCases
            .Invoking(x => x.AutenticarUseCaseAsync(request))
            .Should()
            .ThrowAsync<UsuarioInativoException>()
            .WithMessage("Usuário inativo. Entre em contato com o administrador.");

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComSenhaIncorreta_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoComSenhaInvalida();

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request)
            .Returns(Task.FromException<AutenticacaoDto>(new CredenciaisInvalidasException("Credenciais inválidas")));

        // Act & Assert
        await mockAutenticacaoUseCases
            .Invoking(x => x.AutenticarUseCaseAsync(request))
            .Should()
            .ThrowAsync<CredenciaisInvalidasException>()
            .WithMessage("Credenciais inválidas");

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }

    [Theory]
    [InlineData(TipoUsuario.Admin, "administrador")]
    [InlineData(TipoUsuario.Cliente, "cliente")]
    public async Task AutenticarUseCaseAsync_ComDiferentesTiposDeUsuario_DeveRetornarPermissoesCorretas(
        TipoUsuario tipoUsuario, string permissaoEsperada)
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();
        usuario.TipoUsuario = tipoUsuario;

        if (tipoUsuario == TipoUsuario.Cliente)
        {
            usuario.ClienteId = Guid.NewGuid();
        }

        var autenticacaoEsperada = new AutenticacaoDto
        {
            Token = "token_jwt_valido",
            Usuario = usuario,
            Permissoes = new List<string> { permissaoEsperada }
        };

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request).Returns(autenticacaoEsperada);

        // Act
        var resultado = await mockAutenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Usuario.TipoUsuario.Should().Be(tipoUsuario);
        resultado.Permissoes.Should().NotBeEmpty();
        resultado.Permissoes.Should().Contain(permissaoEsperada);

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComUsuarioAdmin_DeveRetornarPermissoesAdministrador()
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioFuncionario();

        var autenticacaoEsperada = new AutenticacaoDto
        {
            Token = "token_jwt_valido",
            Usuario = usuario,
            Permissoes = new List<string> { "administrador" }
        };

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request).Returns(autenticacaoEsperada);

        // Act
        var resultado = await mockAutenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Usuario.TipoUsuario.Should().Be(TipoUsuario.Admin);
        resultado.Permissoes.Should().Contain("administrador");
        resultado.Permissoes.Should().NotContain("cliente");

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComUsuarioCliente_DeveRetornarPermissoesCliente()
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioCliente();

        var autenticacaoEsperada = new AutenticacaoDto
        {
            Token = "token_jwt_valido",
            Usuario = usuario,
            Permissoes = new List<string> { "cliente" }
        };

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request).Returns(autenticacaoEsperada);

        // Act
        var resultado = await mockAutenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Usuario.TipoUsuario.Should().Be(TipoUsuario.Cliente);
        resultado.Permissoes.Should().Contain("cliente");
        resultado.Permissoes.Should().NotContain("administrador");

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComDadosInvalidos_DeveLancarDadosInvalidosException()
    {
        // Arrange
        var mockAutenticacaoUseCases = _fixture.CriarMockAutenticacaoUseCases();
        var request = new AutenticacaoUseCaseDto
        {
            Email = "email_invalido",
            Senha = ""
        };

        mockAutenticacaoUseCases.AutenticarUseCaseAsync(request)
            .Returns(Task.FromException<AutenticacaoDto>(new DadosInvalidosException("Usuário ou senha inválidos")));

        // Act & Assert
        await mockAutenticacaoUseCases
            .Invoking(x => x.AutenticarUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosInvalidosException>()
            .WithMessage("Usuário ou senha inválidos");

        await mockAutenticacaoUseCases.Received(1).AutenticarUseCaseAsync(request);
    }
}
