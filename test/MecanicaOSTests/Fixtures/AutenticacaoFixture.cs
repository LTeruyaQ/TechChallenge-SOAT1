using Aplicacao.DTOs.Requests.Autenticacao;
using Dominio.Entidades;
using Dominio.Enumeradores;

namespace MecanicaOSTests.Fixtures
{
    public static class AutenticacaoFixture
    {
        public static AutenticacaoRequest CriarAutenticacaoRequestValida()
        {
            return new AutenticacaoRequest
            {
                Email = "usuario@teste.com",
                Senha = "Senha@123"
            };
        }

        public static AutenticacaoRequest CriarAutenticacaoRequestComEmailInvalido()
        {
            return new AutenticacaoRequest
            {
                Email = "emailinvalido",
                Senha = "Senha@123"
            };
        }

        public static Usuario CriarUsuarioAtivo()
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "usuario@teste.com",
                Senha = "hash123",
                Ativo = true,
                TipoUsuario = TipoUsuario.Cliente,
                ClienteId = Guid.NewGuid(),
                RecebeAlertaEstoque = false
            };
        }

        public static Usuario CriarUsuarioInativo()
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "inativo@teste.com",
                Senha = "hash123",
                Ativo = false,
                TipoUsuario = TipoUsuario.Cliente,
                ClienteId = Guid.NewGuid(),
                RecebeAlertaEstoque = false
            };
        }

        public static Usuario CriarUsuarioAdmin()
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "admin@teste.com",
                Senha = "hash123",
                Ativo = true,
                TipoUsuario = TipoUsuario.Admin
            };
        }
    }
}
