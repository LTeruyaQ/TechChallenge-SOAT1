using Core.DTOs.UseCases.Autenticacao;
using Core.DTOs.UseCases.Usuario;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public class AutenticacaoUseCasesFixture : UseCasesFixtureBase
{
    public AutenticacaoUseCases CriarAutenticacaoUseCases(
        IUsuarioUseCases? mockUsuarioUseCases = null,
        IClienteUseCases? mockClienteUseCases = null,
        IServicoSenha? mockServicoSenha = null,
        IServicoJwt? mockServicoJwt = null,
        ILogServico<AutenticacaoUseCases>? mockLogServico = null,
        IUnidadeDeTrabalho? mockUdt = null,
        IUsuarioLogadoServico? mockUsuarioLogado = null)
    {
        mockUsuarioUseCases ??= CriarMockUsuarioUseCases();
        mockClienteUseCases ??= CriarMockClienteUseCases();
        mockServicoSenha ??= CriarMockServicoSenha();
        mockServicoJwt ??= CriarMockServicoJwt();
        mockLogServico ??= CriarMockLogServico<AutenticacaoUseCases>();
        mockUdt ??= CriarMockUnidadeDeTrabalho();
        mockUsuarioLogado ??= CriarMockUsuarioLogadoServico();

        ConfigurarMocksBasicos(mockUdt, mockUsuarioLogado);

        return new AutenticacaoUseCases(
            mockUsuarioUseCases,
            mockServicoSenha,
            mockServicoJwt,
            mockLogServico,
            mockClienteUseCases);
    }

    public static AutenticacaoUseCaseDto CriarAutenticacaoUseCaseDtoValido()
    {
        return new AutenticacaoUseCaseDto
        {
            Email = "admin@mecanicaos.com",
            Senha = "senha123"
        };
    }

    public static AutenticacaoUseCaseDto CriarAutenticacaoUseCaseDtoComEmailInvalido()
    {
        return new AutenticacaoUseCaseDto
        {
            Email = "email_inexistente@mecanicaos.com",
            Senha = "senha123"
        };
    }

    public static AutenticacaoUseCaseDto CriarAutenticacaoUseCaseDtoComSenhaInvalida()
    {
        return new AutenticacaoUseCaseDto
        {
            Email = "admin@mecanicaos.com",
            Senha = "senha_incorreta"
        };
    }

    public static Usuario CriarUsuarioAtivoValido()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "admin@mecanicaos.com",
            Senha = "senha_criptografada",
            TipoUsuario = TipoUsuario.Admin,
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-30),
            DataUltimoAcesso = DateTime.UtcNow.AddHours(-1),
            RecebeAlertaEstoque = true
        };
    }

    public static Usuario CriarUsuarioInativo()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "usuario_inativo@mecanicaos.com",
            Senha = "senha_criptografada",
            TipoUsuario = TipoUsuario.Admin,
            Ativo = false,
            DataCadastro = DateTime.UtcNow.AddDays(-60),
            DataUltimoAcesso = DateTime.UtcNow.AddDays(-30),
            RecebeAlertaEstoque = false
        };
    }

    public static Usuario CriarUsuarioFuncionario()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "funcionario@mecanicaos.com",
            Senha = "senha_criptografada",
            TipoUsuario = TipoUsuario.Admin,
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-15),
            DataUltimoAcesso = DateTime.UtcNow.AddMinutes(-30),
            RecebeAlertaEstoque = false
        };
    }

    public static Usuario CriarUsuarioCliente()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "cliente@mecanicaos.com",
            Senha = "senha_criptografada",
            TipoUsuario = TipoUsuario.Cliente,
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-7),
            DataUltimoAcesso = DateTime.UtcNow.AddHours(-2),
            RecebeAlertaEstoque = false
        };
    }

    public void ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(
        IUsuarioUseCases mockUsuarioUseCases,
        Usuario? usuario = null)
    {
        usuario ??= CriarUsuarioAtivoValido();
        mockUsuarioUseCases.ObterPorEmailUseCaseAsync(Arg.Any<string>()).Returns(usuario);
    }

    public void ConfigurarMockUsuarioUseCasesParaEmailInexistente(
        IUsuarioUseCases mockUsuarioUseCases)
    {
        mockUsuarioUseCases.ObterPorEmailUseCaseAsync(Arg.Any<string>()).Returns((Usuario?)null);
    }

    public void ConfigurarMockServicoSenhaParaSenhaValida(
        IServicoSenha mockServicoSenha)
    {
        mockServicoSenha.VerificarSenha(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
    }

    public void ConfigurarMockServicoSenhaParaSenhaInvalida(
        IServicoSenha mockServicoSenha)
    {
        mockServicoSenha.VerificarSenha(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
    }

    public void ConfigurarMockServicoJwt(
        IServicoJwt mockServicoJwt,
        string token = "token_jwt_valido")
    {
        mockServicoJwt.GerarToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>()).Returns(token);
    }
}
