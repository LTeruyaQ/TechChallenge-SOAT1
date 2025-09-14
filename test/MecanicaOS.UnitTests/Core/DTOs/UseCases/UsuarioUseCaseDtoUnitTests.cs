using Core.DTOs.UseCases.Usuario;
using Core.Enumeradores;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using Xunit;

namespace MecanicaOS.UnitTests.Core.DTOs.UseCases;

public class UsuarioUseCaseDtoUnitTests
{
    [Fact]
    public void CadastrarUsuarioUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = UsuarioUseCaseFixture.CriarCadastrarUsuarioUseCaseDtoCliente();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Email.Should().Be("cliente@teste.com", "o email deve ser armazenado corretamente");
        dto.Senha.Should().Be("senhaSegura123", "a senha deve ser armazenada corretamente");
        dto.TipoUsuario.Should().Be(TipoUsuario.Cliente, "o tipo de usuário deve ser armazenado corretamente");
        dto.RecebeAlertaEstoque.Should().BeFalse("deve ter configuração de alerta definida");
        dto.Documento.Should().Be("12345678901", "o documento deve ser armazenado corretamente");
    }

    [Fact]
    public void CadastrarUsuarioUseCaseDto_DevePermitirUsuarioAdmin()
    {
        // Arrange & Act
        var dto = UsuarioUseCaseFixture.CriarCadastrarUsuarioUseCaseDtoAdmin();

        // Assert
        dto.Email.Should().Be("admin@mecanica.com", "deve aceitar email de administrador");
        dto.TipoUsuario.Should().Be(TipoUsuario.Admin, "deve ser do tipo Admin");
        dto.RecebeAlertaEstoque.Should().BeTrue("admin deve receber alertas de estoque");
        dto.Documento.Should().BeNull("documento pode ser nulo para admin");
    }

    [Fact]
    public void CadastrarUsuarioUseCaseDto_DevePermitirAlertaEstoqueNulo()
    {
        // Arrange & Act
        var dto = UsuarioUseCaseFixture.CriarCadastrarUsuarioUseCaseDtoSemAlerta();

        // Assert
        dto.RecebeAlertaEstoque.Should().BeNull("deve permitir configuração de alerta nula");
        dto.TipoUsuario.Should().Be(TipoUsuario.Admin, "deve manter tipo de usuário");
        dto.Documento.Should().NotBeNullOrEmpty("deve ter documento quando especificado");
    }

    [Fact]
    public void CadastrarUsuarioUseCaseDto_DevePermitirDocumentoNulo()
    {
        // Arrange & Act
        var dto = UsuarioUseCaseFixture.CriarCadastrarUsuarioUseCaseDtoSemDocumento();

        // Assert
        dto.Documento.Should().BeNull("deve permitir documento nulo");
        dto.Email.Should().NotBeNullOrEmpty("email deve estar presente");
        dto.TipoUsuario.Should().Be(TipoUsuario.Cliente, "deve manter tipo de usuário");
    }

    [Theory]
    [InlineData(TipoUsuario.Cliente)]
    [InlineData(TipoUsuario.Admin)]
    public void CadastrarUsuarioUseCaseDto_DeveAceitarTodosTiposUsuario(TipoUsuario tipoUsuario)
    {
        // Arrange & Act
        var dto = new CadastrarUsuarioUseCaseDto
        {
            Email = "teste@email.com",
            Senha = "senha123",
            TipoUsuario = tipoUsuario,
            RecebeAlertaEstoque = true,
            Documento = "12345678901"
        };

        // Assert
        dto.TipoUsuario.Should().Be(tipoUsuario, "deve aceitar o tipo de usuário especificado");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void CadastrarUsuarioUseCaseDto_DeveAceitarConfiguracaoAlerta(bool? recebeAlerta)
    {
        // Arrange & Act
        var dto = new CadastrarUsuarioUseCaseDto
        {
            Email = "teste@email.com",
            Senha = "senha123",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = recebeAlerta,
            Documento = "12345678901"
        };

        // Assert
        dto.RecebeAlertaEstoque.Should().Be(recebeAlerta, "deve aceitar a configuração de alerta especificada");
    }

    [Fact]
    public void AtualizarUsuarioUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = UsuarioUseCaseFixture.CriarAtualizarUsuarioUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Email.Should().Be("usuario.atualizado@email.com", "o email deve ser armazenado corretamente");
        dto.Senha.Should().Be("novaSenhaSegura456", "a senha deve ser armazenada corretamente");
        dto.DataUltimoAcesso.Should().NotBeNull("deve ter data de último acesso");
        dto.TipoUsuario.Should().Be(TipoUsuario.Cliente, "o tipo de usuário deve ser armazenado corretamente");
        dto.RecebeAlertaEstoque.Should().BeTrue("deve ter configuração de alerta definida");
    }

    [Fact]
    public void AtualizarUsuarioUseCaseDto_DevePermitirCamposNulos()
    {
        // Arrange & Act
        var dto = UsuarioUseCaseFixture.CriarAtualizarUsuarioUseCaseDtoComCamposNulos();

        // Assert
        dto.Email.Should().BeNull("deve permitir email nulo");
        dto.Senha.Should().BeNull("deve permitir senha nula");
        dto.DataUltimoAcesso.Should().BeNull("deve permitir data de último acesso nula");
        dto.TipoUsuario.Should().BeNull("deve permitir tipo de usuário nulo");
        dto.RecebeAlertaEstoque.Should().BeNull("deve permitir configuração de alerta nula");
    }

    [Fact]
    public void AtualizarUsuarioUseCaseDto_DevePermitirAtualizacaoParcial()
    {
        // Arrange & Act
        var dto = UsuarioUseCaseFixture.CriarAtualizarUsuarioUseCaseDtoApenasEmail();

        // Assert
        dto.Email.Should().Be("novo.email@teste.com", "deve permitir atualização apenas do email");
        dto.Senha.Should().BeNull("outros campos podem permanecer nulos");
        dto.TipoUsuario.Should().BeNull("outros campos podem permanecer nulos");
    }

    [Fact]
    public void CadastrarUsuarioUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = UsuarioUseCaseFixture.CriarListaCadastrarUsuarioUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(4, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Email), 
            "todos os DTOs devem ter email");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Senha), 
            "todos os DTOs devem ter senha");
    }

    [Fact]
    public void AtualizarUsuarioUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = UsuarioUseCaseFixture.CriarListaAtualizarUsuarioUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
    }

    [Fact]
    public void UsuarioUseCaseDto_DevePermitirAlteracaoDePropriedades()
    {
        // Arrange
        var dto = UsuarioUseCaseFixture.CriarCadastrarUsuarioUseCaseDtoCliente();
        var novoEmail = "novo@email.com";
        var novaSenha = "novaSenha456";

        // Act
        dto.Email = novoEmail;
        dto.Senha = novaSenha;

        // Assert
        dto.Email.Should().Be(novoEmail, "deve permitir alteração do email");
        dto.Senha.Should().Be(novaSenha, "deve permitir alteração da senha");
    }

    [Fact]
    public void UsuarioUseCaseDto_DeveSerDistintoEntreInstancias()
    {
        // Arrange & Act
        var dto1 = UsuarioUseCaseFixture.CriarCadastrarUsuarioUseCaseDtoCliente();
        var dto2 = UsuarioUseCaseFixture.CriarCadastrarUsuarioUseCaseDtoAdmin();

        // Assert
        dto1.Should().NotBeSameAs(dto2, "devem ser instâncias diferentes");
        dto1.Email.Should().NotBe(dto2.Email, "devem ter emails diferentes");
        dto1.TipoUsuario.Should().NotBe(dto2.TipoUsuario, "devem ter tipos diferentes");
    }
}
