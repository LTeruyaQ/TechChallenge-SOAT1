using Core.DTOs.Entidades.Usuarios;
using Core.Entidades;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Fixtures;

public static class UsuarioFixture
{
    public static Usuario CriarUsuarioValido()
    {
        return new Usuario
        {
            Email = "usuario@teste.com",
            Senha = "senha123",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            DataUltimoAcesso = DateTime.Now.AddDays(-1)
        };
    }

    public static Usuario CriarUsuarioClienteValido()
    {
        return new Usuario
        {
            Email = "cliente@teste.com",
            Senha = "senha123",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            ClienteId = Guid.NewGuid(),
            DataUltimoAcesso = DateTime.Now.AddDays(-1)
        };
    }

    public static Usuario CriarUsuarioAdminValido()
    {
        return new Usuario
        {
            Email = "admin@mecanica.com",
            Senha = "adminPass@123",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            DataUltimoAcesso = DateTime.Now.AddHours(-2)
        };
    }

    public static Usuario CriarUsuarioSemUltimoAcesso()
    {
        return new Usuario
        {
            Email = "novo@usuario.com",
            Senha = "novaSenha456",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            ClienteId = Guid.NewGuid()
        };
    }

    public static Usuario CriarUsuarioComParametros(string email, string senha, TipoUsuario tipo, Guid? clienteId = null)
    {
        return new Usuario(email, senha, tipo, clienteId ?? Guid.Empty);
    }

    public static UsuarioEntityDto CriarUsuarioEntityDtoValido()
    {
        return new UsuarioEntityDto
        {
            Id = Guid.NewGuid(),
            Email = "funcionario@mecanica.com",
            Senha = "senhaSegura789",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            ClienteId = Guid.NewGuid(),
            DataUltimoAcesso = DateTime.Now.AddDays(-3),
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-30),
            DataAtualizacao = DateTime.Now.AddDays(-1)
        };
    }

    public static UsuarioEntityDto CriarUsuarioEntityDtoCliente()
    {
        var clienteDto = ClienteFixture.CriarClienteEntityDtoValido();

        return new UsuarioEntityDto
        {
            Id = Guid.NewGuid(),
            Email = "cliente.usuario@email.com",
            Senha = "clientePass123",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            ClienteId = clienteDto.Id,
            Cliente = clienteDto,
            DataUltimoAcesso = DateTime.Now.AddHours(-6),
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-15),
            DataAtualizacao = DateTime.Now.AddDays(-2)
        };
    }

    public static UsuarioEntityDto CriarUsuarioEntityDtoSemCliente()
    {
        return new UsuarioEntityDto
        {
            Id = Guid.NewGuid(),
            Email = "admin.sistema@mecanica.com",
            Senha = "adminSistema@456",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            ClienteId = null,
            Cliente = null,
            DataUltimoAcesso = DateTime.Now.AddMinutes(-30),
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-60),
            DataAtualizacao = DateTime.Now
        };
    }

    public static List<Usuario> CriarListaUsuarios()
    {
        return new List<Usuario>
        {
            CriarUsuarioClienteValido(),
            CriarUsuarioAdminValido(),
            CriarUsuarioSemUltimoAcesso()
        };
    }

    public static List<UsuarioEntityDto> CriarListaUsuarioEntityDto()
    {
        return new List<UsuarioEntityDto>
        {
            CriarUsuarioEntityDtoValido(),
            CriarUsuarioEntityDtoCliente(),
            CriarUsuarioEntityDtoSemCliente()
        };
    }

    public static Usuario CriarUsuarioParaAtualizacao()
    {
        return new Usuario
        {
            Email = "original@email.com",
            Senha = "senhaOriginal",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            DataUltimoAcesso = DateTime.Now.AddDays(-7),
            ClienteId = Guid.NewGuid()
        };
    }
}
