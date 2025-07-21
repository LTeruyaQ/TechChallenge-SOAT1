using Dominio.Entidades;
using Dominio.Enumeradores;
using FluentAssertions;

namespace MecanicaOSTests.Entidades
{
    public class UsuarioTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarUsuario_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var email = "usuario@teste.com";
            var senha = "Senha@123";
            var tipoUsuario = TipoUsuario.Admin;
            var clienteId = Guid.NewGuid();

            // Act
            var usuario = new Usuario(email, senha, tipoUsuario, clienteId);

            // Assert
            usuario.Should().NotBeNull();
            usuario.Email.Should().Be(email);
            usuario.Senha.Should().Be(senha);
            usuario.TipoUsuario.Should().Be(tipoUsuario);
            usuario.ClienteId.Should().Be(clienteId);
            usuario.Ativo.Should().BeTrue();
        }

        [Fact]
        public void Dado_DadosValidos_Quando_Atualizar_Entao_DeveAtualizarComSucesso()
        {
            // Arrange
            var usuario = new Usuario("antigo@email.com", "SenhaAntiga@123", TipoUsuario.Cliente, Guid.NewGuid());
            var novoEmail = "novo@email.com";
            var novoTipoUsuario = TipoUsuario.Admin;
            var novoClienteId = Guid.NewGuid();

            // Act
            usuario.Atualizar(novoEmail, null, null, novoTipoUsuario);

            // Assert
            usuario.Email.Should().Be(novoEmail);
            usuario.TipoUsuario.Should().Be(novoTipoUsuario);
        }

        [Fact]
        public void Dado_DataValida_Quando_AtualizarUltimoAcesso_Entao_DeveAtualizarComSucesso()
        {
            // Arrange
            var usuario = new Usuario("usuario@teste.com", "Senha@123", TipoUsuario.Cliente, Guid.NewGuid());
            var dataAcesso = DateTime.UtcNow.AddHours(-1);

            // Act
            usuario.AtualizarUltimoAcesso(dataAcesso);

            // Assert
            usuario.DataUltimoAcesso.Should().Be(dataAcesso);
        }
    }
}