namespace MecanicaOSTests.Fixtures
{
    public static class UsuarioFixture
    {
        public static CadastrarUsuarioRequest CriarCadastrarUsuarioRequestValido()
        {
            return new CadastrarUsuarioRequest
            {
                Email = "novo.usuario@teste.com",
                Senha = "Senha@123",
                TipoUsuario = TipoUsuario.Cliente,
                Documento = "12345678901",
                RecebeAlertaEstoque = true
            };
        }

        public static AtualizarUsuarioRequest CriarAtualizarUsuarioRequestValido()
        {
            return new AtualizarUsuarioRequest
            {
                Email = "usuario.atualizado@teste.com",
                TipoUsuario = TipoUsuario.Cliente,
                RecebeAlertaEstoque = true
            };
        }

        public static Usuario CriarUsuarioValido()
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "usuario@teste.com",
                Senha = "hash123",
                Ativo = true,
                TipoUsuario = TipoUsuario.Cliente,
                ClienteId = Guid.NewGuid(),
                RecebeAlertaEstoque = true
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
                RecebeAlertaEstoque = false,
                TipoUsuario = TipoUsuario.Admin
            };
        }
    }
}
