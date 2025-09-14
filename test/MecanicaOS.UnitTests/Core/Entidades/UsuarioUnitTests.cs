using Xunit;
using FluentAssertions;
using Core.Entidades;
using Core.Entidades.Abstratos;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class UsuarioUnitTests
{
    [Fact]
    public void Usuario_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var usuario = new Usuario();

        // Assert
        usuario.Should().BeAssignableTo<Entidade>("Usuario deve herdar de Entidade");
        usuario.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        usuario.Ativo.Should().BeTrue("Usuario deve estar ativo por padrão");
        usuario.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        usuario.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void Usuario_QuandoCriadoComParametros_DeveInicializarCorretamente()
    {
        // Arrange
        var email = "usuario@teste.com";
        var senha = "senha123";
        var tipoUsuario = TipoUsuario.Cliente;
        var clienteId = Guid.NewGuid();

        // Act
        var usuario = new Usuario(email, senha, tipoUsuario, clienteId);

        // Assert
        usuario.Email.Should().Be(email, "o email deve ser definido corretamente");
        usuario.Senha.Should().Be(senha, "a senha deve ser definida corretamente");
        usuario.TipoUsuario.Should().Be(tipoUsuario, "o tipo de usuário deve ser definido corretamente");
        usuario.ClienteId.Should().Be(clienteId, "o ClienteId deve ser definido corretamente");
    }

    [Fact]
    public void Usuario_QuandoDefinidoEmail_DeveArmazenarCorretamente()
    {
        // Arrange
        var usuario = new Usuario();
        var emailEsperado = "novo@email.com";

        // Act
        usuario.Email = emailEsperado;

        // Assert
        usuario.Email.Should().Be(emailEsperado, "o email deve ser armazenado corretamente");
    }

    [Fact]
    public void Usuario_QuandoDefinidaSenha_DeveArmazenarCorretamente()
    {
        // Arrange
        var usuario = new Usuario();
        var senhaEsperada = "novaSenha123";

        // Act
        usuario.Senha = senhaEsperada;

        // Assert
        usuario.Senha.Should().Be(senhaEsperada, "a senha deve ser armazenada corretamente");
    }

    [Theory]
    [InlineData(TipoUsuario.Admin)]
    [InlineData(TipoUsuario.Cliente)]
    public void Usuario_QuandoDefinidoTipoUsuario_DeveArmazenarCorretamente(TipoUsuario tipoUsuario)
    {
        // Arrange
        var usuario = new Usuario();

        // Act
        usuario.TipoUsuario = tipoUsuario;

        // Assert
        usuario.TipoUsuario.Should().Be(tipoUsuario, "o tipo de usuário deve ser armazenado corretamente");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Usuario_QuandoDefinidoRecebeAlertaEstoque_DeveArmazenarCorretamente(bool recebeAlerta)
    {
        // Arrange
        var usuario = new Usuario();

        // Act
        usuario.RecebeAlertaEstoque = recebeAlerta;

        // Assert
        usuario.RecebeAlertaEstoque.Should().Be(recebeAlerta, "a configuração de alerta de estoque deve ser armazenada corretamente");
    }

    [Fact]
    public void Usuario_QuandoDefinidaDataUltimoAcesso_DeveArmazenarCorretamente()
    {
        // Arrange
        var usuario = new Usuario();
        var dataEsperada = DateTime.Now;

        // Act
        usuario.DataUltimoAcesso = dataEsperada;

        // Assert
        usuario.DataUltimoAcesso.Should().Be(dataEsperada, "a data do último acesso deve ser armazenada corretamente");
    }

    [Fact]
    public void Usuario_QuandoAtualizadoUltimoAcesso_DeveDefinirDataAtual()
    {
        // Arrange
        var usuario = new Usuario();
        var dataAntes = DateTime.UtcNow;

        // Act
        usuario.AtualizarUltimoAcesso();

        // Assert
        usuario.DataUltimoAcesso.Should().NotBeNull("a data do último acesso deve ser definida");
        usuario.DataUltimoAcesso.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1), "a data deve ser próxima ao momento atual");
    }

    [Fact]
    public void Usuario_QuandoAtualizadoUltimoAcessoComData_DeveDefinirDataEspecifica()
    {
        // Arrange
        var usuario = new Usuario();
        var dataEspecifica = new DateTime(2023, 12, 25, 10, 30, 0);

        // Act
        usuario.AtualizarUltimoAcesso(dataEspecifica);

        // Assert
        usuario.DataUltimoAcesso.Should().Be(dataEspecifica, "a data do último acesso deve ser a data especificada");
    }

    [Fact]
    public void Usuario_QuandoAtualizadoComTodosParametros_DeveAtualizarCorretamente()
    {
        // Arrange
        var usuario = new Usuario();
        var novoEmail = "novo@email.com";
        var novaSenha = "novaSenha123";
        var novaDataAcesso = DateTime.Now;
        var novoTipoUsuario = TipoUsuario.Admin;
        var novoRecebeAlerta = true;
        var novoAtivo = false;

        // Act
        usuario.Atualizar(novoEmail, novaSenha, novaDataAcesso, novoTipoUsuario, novoRecebeAlerta, novoAtivo);

        // Assert
        usuario.Email.Should().Be(novoEmail, "o email deve ser atualizado");
        usuario.Senha.Should().Be(novaSenha, "a senha deve ser atualizada");
        usuario.DataUltimoAcesso.Should().Be(novaDataAcesso, "a data do último acesso deve ser atualizada");
        usuario.TipoUsuario.Should().Be(novoTipoUsuario, "o tipo de usuário deve ser atualizado");
        usuario.RecebeAlertaEstoque.Should().Be(novoRecebeAlerta, "a configuração de alerta deve ser atualizada");
        usuario.Ativo.Should().Be(novoAtivo, "o status ativo deve ser atualizado");
    }

    [Fact]
    public void Usuario_QuandoAtualizadoComParametrosNulos_NaoDeveAlterarValoresExistentes()
    {
        // Arrange
        var usuario = new Usuario
        {
            Email = "original@email.com",
            Senha = "senhaOriginal",
            DataUltimoAcesso = DateTime.Now.AddDays(-1),
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            Ativo = true
        };

        var valoresOriginais = new
        {
            Email = usuario.Email,
            Senha = usuario.Senha,
            DataUltimoAcesso = usuario.DataUltimoAcesso,
            TipoUsuario = usuario.TipoUsuario,
            RecebeAlertaEstoque = usuario.RecebeAlertaEstoque,
            Ativo = usuario.Ativo
        };

        // Act
        usuario.Atualizar();

        // Assert
        usuario.Email.Should().Be(valoresOriginais.Email, "o email não deve ser alterado");
        usuario.Senha.Should().Be(valoresOriginais.Senha, "a senha não deve ser alterada");
        usuario.DataUltimoAcesso.Should().Be(valoresOriginais.DataUltimoAcesso, "a data do último acesso não deve ser alterada");
        usuario.TipoUsuario.Should().Be(valoresOriginais.TipoUsuario, "o tipo de usuário não deve ser alterado");
        usuario.RecebeAlertaEstoque.Should().Be(valoresOriginais.RecebeAlertaEstoque, "a configuração de alerta não deve ser alterada");
        usuario.Ativo.Should().Be(valoresOriginais.Ativo, "o status ativo não deve ser alterado");
    }

    [Fact]
    public void Usuario_QuandoComparadoComOutroUsuarioComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var usuario1 = new Usuario { Id = id, Email = "email1@teste.com" };
        var usuario2 = new Usuario { Id = id, Email = "email2@teste.com" };

        // Act & Assert
        usuario1.Should().Be(usuario2, "usuários com mesmo Id devem ser considerados iguais");
        usuario1.GetHashCode().Should().Be(usuario2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }
}
