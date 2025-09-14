namespace MecanicaOSTests.Dominio.Especificacoes.Usuario
{
    public class UsuarioEspecificacaoTests
    {
        private List<global::Dominio.Entidades.Usuario> GetUsuariosDeTeste()
        {
            return new List<global::Dominio.Entidades.Usuario>
            {
                new global::Dominio.Entidades.Usuario { Id = Guid.NewGuid(), Email = "admin1@teste.com", TipoUsuario = TipoUsuario.Admin, RecebeAlertaEstoque = true, Ativo = true },
                new global::Dominio.Entidades.Usuario { Id = Guid.NewGuid(), Email = "admin2@teste.com", TipoUsuario = TipoUsuario.Admin, RecebeAlertaEstoque = true, Ativo = false }, // Inativo, mas recebe alerta
                new global::Dominio.Entidades.Usuario { Id = Guid.NewGuid(), Email = "cliente@teste.com", TipoUsuario = TipoUsuario.Cliente, RecebeAlertaEstoque = false, Ativo = true },
                new global::Dominio.Entidades.Usuario { Id = Guid.NewGuid(), Email = "admin3@teste.com", TipoUsuario = TipoUsuario.Admin, RecebeAlertaEstoque = true, Ativo = true }
            };
        }

        [Fact]
        public void ObterUsuarioParaAlertaEstoqueEspecificacao_DeveRetornarUsuariosCorretos()
        {
            // Arrange
            var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();
            var usuarios = GetUsuariosDeTeste();

            // Act
            var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Equal(3, resultado.Count);
            Assert.All(resultado, u => Assert.True(u.RecebeAlertaEstoque));
        }

        [Fact]
        public void ObterUsuarioPorEmailEspecificacao_DeveRetornarUsuarioCorreto()
        {
            // Arrange
            var email = "admin1@teste.com";
            var especificacao = new ObterUsuarioPorEmailEspecificacao(email);
            var usuarios = GetUsuariosDeTeste();

            // Act
            var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(email, resultado.First().Email);
        }
    }
}
