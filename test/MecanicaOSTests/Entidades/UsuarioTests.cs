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

        [Fact]
        public void Dado_ConstrutorSemParametros_Quando_CriarUsuario_Entao_DeveCriarComSucesso()
        {
            // Act
            var usuario = new Usuario();

            // Assert
            usuario.Should().NotBeNull();
            usuario.Id.Should().NotBeEmpty();
            usuario.Ativo.Should().BeTrue();
        }

        [Fact]
        public void Quando_AtualizarUltimoAcessoSemParametros_Entao_DeveAtualizarParaDataAtual()
        {
            // Arrange
            var usuario = new Usuario();
            usuario.AtualizarUltimoAcesso(DateTime.UtcNow.AddMinutes(-1));
            var dataAntiga = usuario.DataUltimoAcesso;

            // Act
            usuario.AtualizarUltimoAcesso();

            // Assert
            usuario.DataUltimoAcesso.Should().NotBeNull();
            usuario.DataUltimoAcesso.Should().BeAfter(dataAntiga.Value);
        }

        [Fact]
        public void Dado_TodosDadosValidos_Quando_Atualizar_Entao_DeveAtualizarTodosCampos()
        {
            // Arrange
            var usuario = new Usuario("antigo@email.com", "SenhaAntiga@123", TipoUsuario.Cliente, Guid.NewGuid());
            var novoEmail = "novo@email.com";
            var novaSenha = "NovaSenha@123";
            var novaDataUltimoAcesso = DateTime.UtcNow;
            var novoTipoUsuario = TipoUsuario.Admin;
            var novoRecebeAlerta = true;
            var novoAtivo = false;

            // Act
            usuario.Atualizar(novoEmail, novaSenha, novaDataUltimoAcesso, novoTipoUsuario, novoRecebeAlerta, novoAtivo);

            // Assert
            usuario.Email.Should().Be(novoEmail);
            usuario.Senha.Should().Be(novaSenha);
            usuario.DataUltimoAcesso.Should().Be(novaDataUltimoAcesso);
            usuario.TipoUsuario.Should().Be(novoTipoUsuario);
            usuario.RecebeAlertaEstoque.Should().Be(novoRecebeAlerta);
            usuario.Ativo.Should().Be(novoAtivo);
        }

        [Fact]
        public void Dado_UsuarioComCliente_Quando_AcessarCliente_Entao_DeveRetornarCliente()
        {
            //Arrange
            var cliente = new Cliente();
            var usuario = new Usuario("teste@teste.com", "senha", TipoUsuario.Cliente, cliente.Id)
            {
                Cliente = cliente
            };

            //Act
            var clienteDoUsuario = usuario.Cliente;

            //Assert
            Assert.NotNull(clienteDoUsuario);
            Assert.Equal(cliente, clienteDoUsuario);
        }
    }
}