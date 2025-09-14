using Core.DTOs.UseCases.Autenticacao;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Servicos;
using Core.UseCases;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class AutenticacaoUseCasesUnitTests
{
    private readonly AutenticacaoUseCasesFixture _fixture;

    public AutenticacaoUseCasesUnitTests()
    {
        _fixture = new AutenticacaoUseCasesFixture();
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComCredenciaisValidas_DeveRetornarAutenticacaoDto()
    {
        // Arrange
        var mockUsuarioUseCases = _fixture.CriarMockUsuarioUseCases();
        var mockServicoSenha = _fixture.CriarMockServicoSenha();
        var mockServicoJwt = _fixture.CriarMockServicoJwt();
        
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();
        var tokenEsperado = "token_jwt_gerado";

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(mockUsuarioUseCases, usuario);
        _fixture.ConfigurarMockServicoSenhaParaSenhaValida(mockServicoSenha);
        _fixture.ConfigurarMockServicoJwt(mockServicoJwt, tokenEsperado);

        var autenticacaoUseCases = _fixture.CriarAutenticacaoUseCases(
            mockUsuarioUseCases, null, mockServicoSenha, mockServicoJwt);

        // Act
        var resultado = await autenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().Be(tokenEsperado);
        resultado.Usuario.Should().NotBeNull();
        resultado.Usuario.Email.Should().Be(usuario.Email);
        resultado.Permissoes.Should().NotBeEmpty();

        mockUsuarioUseCases.Received(1).ObterPorEmailUseCaseAsync(request.Email);
        mockServicoSenha.Received(1).VerificarSenha(request.Senha, usuario.Senha);
        mockServicoJwt.Received(1).GerarToken(usuario.Id, usuario.Email, usuario.TipoUsuario.ToString(), usuario.Email, Arg.Any<IEnumerable<string>>());
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComEmailInexistente_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange
        var mockUsuarioUseCases = _fixture.CriarMockUsuarioUseCases();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoComEmailInvalido();

        _fixture.ConfigurarMockUsuarioUseCasesParaEmailInexistente(mockUsuarioUseCases);

        var autenticacaoUseCases = _fixture.CriarAutenticacaoUseCases(mockUsuarioUseCases);

        // Act & Assert
        await autenticacaoUseCases
            .Invoking(x => x.AutenticarUseCaseAsync(request))
            .Should()
            .ThrowAsync<CredenciaisInvalidasException>()
            .WithMessage("Credenciais inválidas");

        mockUsuarioUseCases.Received(1).ObterPorEmailUseCaseAsync(request.Email);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComUsuarioInativo_DeveLancarUsuarioInativoException()
    {
        // Arrange
        var mockUsuarioUseCases = _fixture.CriarMockUsuarioUseCases();
        var usuarioInativo = AutenticacaoUseCasesFixture.CriarUsuarioInativo();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(mockUsuarioUseCases, usuarioInativo);

        var autenticacaoUseCases = _fixture.CriarAutenticacaoUseCases(mockUsuarioUseCases);

        // Act & Assert
        await autenticacaoUseCases
            .Invoking(x => x.AutenticarUseCaseAsync(request))
            .Should()
            .ThrowAsync<UsuarioInativoException>()
            .WithMessage("Usuário inativo. Entre em contato com o administrador.");

        mockUsuarioUseCases.Received(1).ObterPorEmailUseCaseAsync(request.Email);
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComSenhaIncorreta_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange
        var mockUsuarioUseCases = _fixture.CriarMockUsuarioUseCases();
        var mockServicoSenha = _fixture.CriarMockServicoSenha();
        
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoComSenhaInvalida();

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(mockUsuarioUseCases, usuario);
        _fixture.ConfigurarMockServicoSenhaParaSenhaInvalida(mockServicoSenha);

        var autenticacaoUseCases = _fixture.CriarAutenticacaoUseCases(
            mockUsuarioUseCases, null, mockServicoSenha);

        // Act & Assert
        await autenticacaoUseCases
            .Invoking(x => x.AutenticarUseCaseAsync(request))
            .Should()
            .ThrowAsync<CredenciaisInvalidasException>()
            .WithMessage("Credenciais inválidas");

        mockServicoSenha.Received(1).VerificarSenha(request.Senha, usuario.Senha);
    }

    [Theory]
    [InlineData(TipoUsuario.Admin)]
    [InlineData(TipoUsuario.Cliente)]
    public async Task AutenticarUseCaseAsync_ComDiferentesTiposDeUsuario_DeveRetornarPermissoesCorretas(TipoUsuario tipoUsuario)
    {
        // Arrange
        var mockUsuarioUseCases = _fixture.CriarMockUsuarioUseCases();
        var mockServicoSenha = _fixture.CriarMockServicoSenha();
        var mockServicoJwt = _fixture.CriarMockServicoJwt();
        var mockClienteUseCases = _fixture.CriarMockClienteUseCases();
        
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();
        usuario.TipoUsuario = tipoUsuario;
        
        // Adicionar ClienteId se for usuário do tipo Cliente
        if (tipoUsuario == TipoUsuario.Cliente)
        {
            usuario.ClienteId = Guid.NewGuid();
            _fixture.ConfigurarMockClienteUseCasesParaClienteValido(mockClienteUseCases, usuario.ClienteId.Value);
        }
        
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(mockUsuarioUseCases, usuario);
        _fixture.ConfigurarMockServicoSenhaParaSenhaValida(mockServicoSenha);
        _fixture.ConfigurarMockServicoJwt(mockServicoJwt);

        var autenticacaoUseCases = _fixture.CriarAutenticacaoUseCases(
            mockUsuarioUseCases, mockClienteUseCases, mockServicoSenha, mockServicoJwt);

        // Act
        var resultado = await autenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Usuario.TipoUsuario.Should().Be(tipoUsuario);
        resultado.Permissoes.Should().NotBeEmpty();
        
        // Verificar permissões específicas por tipo de usuário
        switch (tipoUsuario)
        {
            case TipoUsuario.Admin:
                resultado.Permissoes.Should().Contain("administrador");
                break;
            case TipoUsuario.Cliente:
                resultado.Permissoes.Should().Contain("cliente");
                resultado.Permissoes.Should().NotContain("administrador");
                break;
        }
    }

    [Fact]
    public void Constructor_ComParametrosNulos_DeveLancarArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new AutenticacaoUseCases(
                null!, 
                _fixture.CriarMockServicoSenha(), 
                _fixture.CriarMockServicoJwt(), 
                _fixture.CriarMockLogServico<AutenticacaoUseCases>(),
                _fixture.CriarMockClienteUseCases()));
        
        Assert.Throws<ArgumentNullException>(() => 
            new AutenticacaoUseCases(
                _fixture.CriarMockUsuarioUseCases(), 
                null!, 
                _fixture.CriarMockServicoJwt(), 
                _fixture.CriarMockLogServico<AutenticacaoUseCases>(),
                _fixture.CriarMockClienteUseCases()));
        
        Assert.Throws<ArgumentNullException>(() => 
            new AutenticacaoUseCases(
                _fixture.CriarMockUsuarioUseCases(), 
                _fixture.CriarMockServicoSenha(), 
                null!, 
                _fixture.CriarMockLogServico<AutenticacaoUseCases>(),
                _fixture.CriarMockClienteUseCases()));
                
        Assert.Throws<ArgumentNullException>(() => 
            new AutenticacaoUseCases(
                _fixture.CriarMockUsuarioUseCases(), 
                _fixture.CriarMockServicoSenha(), 
                _fixture.CriarMockServicoJwt(), 
                null!,
                _fixture.CriarMockClienteUseCases()));
                
        Assert.Throws<ArgumentNullException>(() => 
            new AutenticacaoUseCases(
                _fixture.CriarMockUsuarioUseCases(), 
                _fixture.CriarMockServicoSenha(), 
                _fixture.CriarMockServicoJwt(), 
                _fixture.CriarMockLogServico<AutenticacaoUseCases>(),
                null!));
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComUsuarioFuncionario_DeveRetornarPermissoesFuncionario()
    {
        // Arrange
        var mockUsuarioUseCases = _fixture.CriarMockUsuarioUseCases();
        var mockServicoSenha = _fixture.CriarMockServicoSenha();
        var mockServicoJwt = _fixture.CriarMockServicoJwt();
        
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioFuncionario();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(mockUsuarioUseCases, usuario);
        _fixture.ConfigurarMockServicoSenhaParaSenhaValida(mockServicoSenha);
        _fixture.ConfigurarMockServicoJwt(mockServicoJwt);

        var autenticacaoUseCases = _fixture.CriarAutenticacaoUseCases(
            mockUsuarioUseCases, null, mockServicoSenha, mockServicoJwt);

        // Act
        var resultado = await autenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Usuario.TipoUsuario.Should().Be(TipoUsuario.Admin);
        resultado.Permissoes.Should().Contain("administrador");
    }

    [Fact]
    public async Task AutenticarUseCaseAsync_ComUsuarioCliente_DeveRetornarPermissoesCliente()
    {
        // Arrange
        var mockUsuarioUseCases = _fixture.CriarMockUsuarioUseCases();
        var mockServicoSenha = _fixture.CriarMockServicoSenha();
        var mockServicoJwt = _fixture.CriarMockServicoJwt();
        var mockClienteUseCases = _fixture.CriarMockClienteUseCases();
        
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioCliente();
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(mockUsuarioUseCases, usuario);
        _fixture.ConfigurarMockServicoSenhaParaSenhaValida(mockServicoSenha);
        _fixture.ConfigurarMockServicoJwt(mockServicoJwt);
        _fixture.ConfigurarMockClienteUseCasesParaClienteValido(mockClienteUseCases, usuario.ClienteId!.Value, "Cliente Teste");

        var autenticacaoUseCases = _fixture.CriarAutenticacaoUseCases(
            mockUsuarioUseCases, mockClienteUseCases, mockServicoSenha, mockServicoJwt);

        // Act
        var resultado = await autenticacaoUseCases.AutenticarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Usuario.TipoUsuario.Should().Be(TipoUsuario.Cliente);
        resultado.Permissoes.Should().Contain("cliente");
        resultado.Permissoes.Should().NotContain("administrador");
    }
}
