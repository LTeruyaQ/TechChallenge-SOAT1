using Core.DTOs.UseCases.Usuario;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public static class UsuarioUseCaseFixture
{
    public static CadastrarUsuarioUseCaseDto CriarCadastrarUsuarioUseCaseDtoCliente()
    {
        return new CadastrarUsuarioUseCaseDto
        {
            Email = "cliente@teste.com",
            Senha = "senhaSegura123",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            Documento = "12345678901"
        };
    }

    public static CadastrarUsuarioUseCaseDto CriarCadastrarUsuarioUseCaseDtoAdmin()
    {
        return new CadastrarUsuarioUseCaseDto
        {
            Email = "admin@mecanica.com",
            Senha = "adminPass@456",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            Documento = null
        };
    }

    public static CadastrarUsuarioUseCaseDto CriarCadastrarUsuarioUseCaseDtoSemAlerta()
    {
        return new CadastrarUsuarioUseCaseDto
        {
            Email = "funcionario@mecanica.com",
            Senha = "funcPass789",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = null,
            Documento = "98765432100"
        };
    }

    public static CadastrarUsuarioUseCaseDto CriarCadastrarUsuarioUseCaseDtoSemDocumento()
    {
        return new CadastrarUsuarioUseCaseDto
        {
            Email = "usuario@sistema.com",
            Senha = "userPass321",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            Documento = null
        };
    }

    public static AtualizarUsuarioUseCaseDto CriarAtualizarUsuarioUseCaseDtoValido()
    {
        return new AtualizarUsuarioUseCaseDto
        {
            Email = "usuario.atualizado@email.com",
            Senha = "novaSenhaSegura456",
            DataUltimoAcesso = DateTime.Now.AddDays(-1),
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = true
        };
    }

    public static AtualizarUsuarioUseCaseDto CriarAtualizarUsuarioUseCaseDtoComCamposNulos()
    {
        return new AtualizarUsuarioUseCaseDto
        {
            Email = null,
            Senha = null,
            DataUltimoAcesso = null,
            TipoUsuario = null,
            RecebeAlertaEstoque = null
        };
    }

    public static AtualizarUsuarioUseCaseDto CriarAtualizarUsuarioUseCaseDtoApenasEmail()
    {
        return new AtualizarUsuarioUseCaseDto
        {
            Email = "novo.email@teste.com"
        };
    }

    public static List<CadastrarUsuarioUseCaseDto> CriarListaCadastrarUsuarioUseCaseDto()
    {
        return new List<CadastrarUsuarioUseCaseDto>
        {
            CriarCadastrarUsuarioUseCaseDtoCliente(),
            CriarCadastrarUsuarioUseCaseDtoAdmin(),
            CriarCadastrarUsuarioUseCaseDtoSemAlerta(),
            CriarCadastrarUsuarioUseCaseDtoSemDocumento()
        };
    }

    public static List<AtualizarUsuarioUseCaseDto> CriarListaAtualizarUsuarioUseCaseDto()
    {
        return new List<AtualizarUsuarioUseCaseDto>
        {
            CriarAtualizarUsuarioUseCaseDtoValido(),
            CriarAtualizarUsuarioUseCaseDtoComCamposNulos(),
            CriarAtualizarUsuarioUseCaseDtoApenasEmail()
        };
    }
}
