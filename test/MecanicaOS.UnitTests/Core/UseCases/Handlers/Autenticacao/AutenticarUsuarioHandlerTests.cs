using Core.DTOs.UseCases.Autenticacao;
using Core.Enumeradores;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Autenticacao;

public class AutenticarUsuarioHandlerTests
{
    private readonly AutenticacaoHandlerFixture _fixture;

    public AutenticarUsuarioHandlerTests()
    {
        _fixture = new AutenticacaoHandlerFixture();
    }

    [Fact]
    public async Task Handle_ComCredenciaisValidas_DeveRetornarAutenticacaoDto()
    {
        // Arrange
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoValido();
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(usuario);
        _fixture.ConfigurarMockSegurancaGatewayParaSenhaValida();
        _fixture.ConfigurarMockSegurancaGatewayParaGerarToken("token_jwt_valido");

        var handler = _fixture.CriarAutenticarUsuarioHandler();

        // Act
        var resultado = await handler.Handle(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().Be("token_jwt_valido");
        resultado.Usuario.Should().NotBeNull();
        resultado.Usuario.Email.Should().Be(usuario.Email);
        resultado.Permissoes.Should().NotBeEmpty();
        resultado.Permissoes.Should().Contain("administrador");

        // Verificar que os serviços foram chamados
        _fixture.SegurancaGateway.Received(1).VerificarSenha(request.Senha, usuario.Senha);
        _fixture.SegurancaGateway.Received(1).GerarToken(
            Arg.Any<Guid>(),
            usuario.Email,
            usuario.TipoUsuario.ToString(),
            Arg.Any<Guid?>(),
            Arg.Is<IEnumerable<string>>(p => p.Contains("administrador")));

        // Verificar que os logs foram registrados
        _fixture.LogAutenticacao.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<AutenticacaoUseCaseDto>());
        _fixture.LogAutenticacao.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
    }

    [Fact]
    public async Task Handle_ComEmailInexistente_DeveLancarDadosInvalidosException()
    {
        // Arrange
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoComEmailInvalido();

        _fixture.ConfigurarMockUsuarioUseCasesParaEmailInexistente();

        var handler = _fixture.CriarAutenticarUsuarioHandler();

        // Act & Assert
        var act = async () => await handler.Handle(request);

        await act.Should().ThrowAsync<DadosInvalidosException>()
            .WithMessage("Usuário ou senha inválidos");

        // Verificar que os serviços foram chamados
        _fixture.SegurancaGateway.DidNotReceive().VerificarSenha(Arg.Any<string>(), Arg.Any<string>());
        _fixture.SegurancaGateway.DidNotReceive().GerarToken(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<Guid?>(),
            Arg.Any<IEnumerable<string>>());

        // Verificar que os logs foram registrados
        _fixture.LogAutenticacao.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<AutenticacaoUseCaseDto>());
        _fixture.LogAutenticacao.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
    }

    [Fact]
    public async Task Handle_ComSenhaIncorreta_DeveLancarDadosInvalidosException()
    {
        // Arrange
        var request = AutenticacaoUseCasesFixture.CriarAutenticacaoUseCaseDtoComSenhaInvalida();
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(usuario);
        _fixture.ConfigurarMockSegurancaGatewayParaSenhaInvalida();

        var handler = _fixture.CriarAutenticarUsuarioHandler();

        // Act & Assert
        var act = async () => await handler.Handle(request);

        await act.Should().ThrowAsync<DadosInvalidosException>()
            .WithMessage("Usuário ou senha inválidos");

        // Verificar que os serviços foram chamados
        _fixture.SegurancaGateway.Received(1).VerificarSenha(request.Senha, usuario.Senha);
        _fixture.SegurancaGateway.DidNotReceive().GerarToken(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<Guid?>(),
            Arg.Any<IEnumerable<string>>());

        // Verificar que os logs foram registrados
        _fixture.LogAutenticacao.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<AutenticacaoUseCaseDto>());
        _fixture.LogAutenticacao.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
    }

    [Theory]
    [InlineData(TipoUsuario.Admin, "administrador")]
    [InlineData(TipoUsuario.Cliente, "cliente")]
    public async Task Handle_ComDiferentesTiposDeUsuario_DeveRetornarPermissoesCorretas(
        TipoUsuario tipoUsuario, string permissaoEsperada)
    {
        // Arrange
        var usuario = AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();
        usuario.TipoUsuario = tipoUsuario;

        if (tipoUsuario == TipoUsuario.Cliente)
        {
            usuario.ClienteId = Guid.NewGuid();
            _fixture.ConfigurarMockClienteUseCasesParaClienteValido(usuario.ClienteId.Value);
        }

        var request = new AutenticacaoUseCaseDto
        {
            Email = "admin@mecanicaos.com",
            Senha = "senha123",
            UsuarioExistente = usuario
        };

        _fixture.ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(usuario);
        _fixture.ConfigurarMockSegurancaGatewayParaSenhaValida();
        _fixture.ConfigurarMockSegurancaGatewayParaGerarToken("token_jwt_valido");

        var handler = _fixture.CriarAutenticarUsuarioHandler();

        // Act
        var resultado = await handler.Handle(request);

        // Assert
        resultado.Usuario.TipoUsuario.Should().Be(tipoUsuario);
        resultado.Permissoes.Should().NotBeEmpty();
        resultado.Permissoes.Should().Contain(permissaoEsperada);

        // Verificar que os serviços foram chamados
        _fixture.SegurancaGateway.Received(1).VerificarSenha(request.Senha, usuario.Senha);
        _fixture.SegurancaGateway.Received(1).GerarToken(
            Arg.Any<Guid>(),
            usuario.Email,
            usuario.TipoUsuario.ToString(),
            Arg.Any<Guid?>(),
            Arg.Is<IEnumerable<string>>(p => p.Contains(permissaoEsperada)));

        // Verificar que os logs foram registrados
        _fixture.LogAutenticacao.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<AutenticacaoUseCaseDto>());
        _fixture.LogAutenticacao.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
    }

    [Fact]
    public async Task Handle_ComDadosInvalidos_DeveLancarDadosInvalidosException()
    {
        // Arrange
        var request = new AutenticacaoUseCaseDto
        {
            Email = null,
            Senha = null
        };

        var handler = _fixture.CriarAutenticarUsuarioHandler();

        // Act & Assert
        var act = async () => await handler.Handle(request);

        await act.Should().ThrowAsync<DadosInvalidosException>();

        // Verificar que os logs foram registrados
        _fixture.LogAutenticacao.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<AutenticacaoUseCaseDto>());
        _fixture.LogAutenticacao.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
    }
}
