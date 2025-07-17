using Xunit;
using Dominio.Entidades;
using Dominio.Enumeradores;
using MecanicaOSTests.Fixtures;
using FluentAssertions;

namespace Dominio.Entidades.Tests
{
    public class UsuarioTests
    {
        [Fact]
        public void Construtor_QuandoInvocado_DeveCriarUsuarioComValoresPadrao()
        {
            // Arrange & Act
            var usuario = new Usuario();

            // Assert
            usuario.Id.Should().NotBe(Guid.Empty, "porque todo usuário deve ter um ID único");
            usuario.Email.Should().BeEmpty("porque o email não foi definido no construtor");
            usuario.Senha.Should().BeEmpty("porque a senha não foi definida no construtor");
            usuario.DataUltimoAcesso.Should().BeNull("porque o último acesso não foi definido");
            usuario.RecebeAlertaEstoque.Should().BeFalse("porque o alerta de estoque deve ser falso por padrão");
            usuario.Ativo.Should().BeTrue("porque todo usuário deve ser ativo por padrão");
        }

        [Fact]
        public void Atualizar_QuandoDadosValidos_DeveAtualizarPropriedades()
        {
            // Arrange
            var usuario = UsuarioFixture.CriarUsuarioValido();
            var novoEmail = "novo.email@teste.com";
            var novaSenha = "NovaSenha@123";
            var dataAcesso = DateTime.UtcNow.AddDays(-1);
            var novoStatus = false;

            // Act
            usuario.Atualizar(
                email: novoEmail,
                senha: novaSenha,
                dataUltimoAcesso: dataAcesso,
                tipoUsuario: TipoUsuario.Admin,
                recebeAlertaEstoque: false,
                ativo: novoStatus);

            // Assert
            usuario.Email.Should().Be(novoEmail, "porque o email deve ser atualizado");
            usuario.Senha.Should().Be(novaSenha, "porque a senha deve ser atualizada");
            usuario.DataUltimoAcesso.Should().Be(dataAcesso, "porque a data de último acesso deve ser atualizada");
            usuario.TipoUsuario.Should().Be(TipoUsuario.Admin, "porque o tipo de usuário deve ser atualizado");
            usuario.RecebeAlertaEstoque.Should().BeFalse("porque o alerta de estoque deve ser atualizado");
            usuario.Ativo.Should().Be(novoStatus, "porque o status ativo deve ser atualizado");
        }

        [Fact]
        public void Atualizar_QuandoDadosNulos_DeveManterValoresAtuais()
        {
            // Arrange
            var usuario = UsuarioFixture.CriarUsuarioValido();
            var emailOriginal = usuario.Email;
            var senhaOriginal = usuario.Senha;
            var tipoOriginal = usuario.TipoUsuario;
            var alertaEstoqueOriginal = usuario.RecebeAlertaEstoque;
            var ativoOriginal = usuario.Ativo;

            // Act
            usuario.Atualizar(email: null, senha: null, null, null, null, null);

            // Assert
            usuario.Email.Should().Be(emailOriginal, "porque o email não deve ser alterado quando nulo");
            usuario.Senha.Should().Be(senhaOriginal, "porque a senha não deve ser alterada quando nula");
            usuario.TipoUsuario.Should().Be(tipoOriginal, "porque o tipo de usuário não deve ser alterado quando nulo");
            usuario.RecebeAlertaEstoque.Should().Be(alertaEstoqueOriginal, "porque o alerta de estoque não deve ser alterado quando nulo");
            usuario.Ativo.Should().Be(ativoOriginal, "porque o status ativo não deve ser alterado quando nulo");
        }

        [Fact]
        public void AtualizarUltimoAcesso_QuandoChamado_DeveAtualizarDataUltimoAcesso()
        {
            // Arrange
            var usuario = UsuarioFixture.CriarUsuarioValido();
            var dataAntes = DateTime.UtcNow;
            
            // Act
            usuario.AtualizarUltimoAcesso();
            var dataDepois = DateTime.UtcNow;

            // Assert
            usuario.DataUltimoAcesso.Should().NotBeNull("porque a data de último acesso deve ser definida");
            usuario.DataUltimoAcesso.Should().BeOnOrAfter(dataAntes, "porque a data de acesso deve ser igual ou posterior ao momento antes da chamada");
            usuario.DataUltimoAcesso.Should().BeOnOrBefore(dataDepois, "porque a data de acesso deve ser igual ou anterior ao momento após a chamada");
        }

        [Fact]
        public void Usuario_QuandoCliente_DeveTerClienteIdPreenchido()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            
            // Act
            var usuario = new Usuario
            {
                TipoUsuario = TipoUsuario.Cliente,
                ClienteId = clienteId
            };

            // Assert
            usuario.ClienteId.Should().Be(clienteId, "porque o ID do cliente deve ser o mesmo que foi definido");
            usuario.TipoUsuario.Should().Be(TipoUsuario.Cliente, "porque o tipo de usuário deve ser Cliente");
        }

        [Fact]
        public void Usuario_QuandoAdmin_ClienteIdPodeSerNulo()
        {
            // Arrange & Act
            var usuario = new Usuario
            {
                TipoUsuario = TipoUsuario.Admin,
                ClienteId = null
            };

            // Assert
            usuario.ClienteId.Should().BeNull("porque usuários administradores não precisam ter um cliente associado");
            usuario.TipoUsuario.Should().Be(TipoUsuario.Admin, "porque o tipo de usuário deve ser Admin");
        }
    }
}