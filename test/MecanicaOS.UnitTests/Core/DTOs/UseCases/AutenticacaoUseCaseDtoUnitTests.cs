using Core.DTOs.UseCases.Autenticacao;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.DTOs.UseCases;

public class AutenticacaoUseCaseDtoUnitTests
{
    [Fact]
    public void AutenticacaoUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = AutenticacaoUseCaseFixture.CriarAutenticacaoUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Email.Should().Be("usuario@teste.com", "o email deve ser armazenado corretamente");
        dto.Senha.Should().Be("senhaSegura123", "a senha deve ser armazenada corretamente");
    }

    [Fact]
    public void AutenticacaoUseCaseDto_DevePermitirEmailAdmin()
    {
        // Arrange & Act
        var dto = AutenticacaoUseCaseFixture.CriarAutenticacaoUseCaseDtoAdmin();

        // Assert
        dto.Email.Should().Be("admin@mecanica.com", "deve aceitar email de administrador");
        dto.Senha.Should().Be("adminPass@456", "deve aceitar senha de administrador");
    }

    [Fact]
    public void AutenticacaoUseCaseDto_DevePermitirEmailLongo()
    {
        // Arrange & Act
        var dto = AutenticacaoUseCaseFixture.CriarAutenticacaoUseCaseDtoEmailLongo();

        // Assert
        dto.Email.Should().Be("usuario.com.nome.muito.longo@empresa.exemplo.com.br",
            "deve aceitar emails longos");
        dto.Senha.Should().NotBeNullOrEmpty("senha deve estar presente");
    }

    [Fact]
    public void AutenticacaoUseCaseDto_DevePermitirSenhaComplexa()
    {
        // Arrange & Act
        var dto = AutenticacaoUseCaseFixture.CriarAutenticacaoUseCaseDtoSenhaCompleta();

        // Assert
        dto.Senha.Should().Be("MinhaSenh@Muito$egura123!@#",
            "deve aceitar senhas com caracteres especiais");
        dto.Email.Should().NotBeNullOrEmpty("email deve estar presente");
    }

    [Theory]
    [InlineData("", "senha123")]
    [InlineData("email@teste.com", "")]
    [InlineData("", "")]
    public void AutenticacaoUseCaseDto_DevePermitirCamposVazios(string email, string senha)
    {
        // Arrange & Act
        var dto = new AutenticacaoUseCaseDto
        {
            Email = email,
            Senha = senha
        };

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado mesmo com campos vazios");
        dto.Email.Should().Be(email, "deve armazenar o email fornecido");
        dto.Senha.Should().Be(senha, "deve armazenar a senha fornecida");
    }

    [Fact]
    public void AutenticacaoUseCaseDto_DevePermitirAlteracaoDePropriedades()
    {
        // Arrange
        var dto = AutenticacaoUseCaseFixture.CriarAutenticacaoUseCaseDtoValido();
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
    public void AutenticacaoUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = AutenticacaoUseCaseFixture.CriarListaAutenticacaoUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(4, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Email),
            "todos os DTOs devem ter email");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Senha),
            "todos os DTOs devem ter senha");
    }

    [Fact]
    public void AutenticacaoUseCaseDto_DeveSerDistintoEntreInstancias()
    {
        // Arrange & Act
        var dto1 = AutenticacaoUseCaseFixture.CriarAutenticacaoUseCaseDtoValido();
        var dto2 = AutenticacaoUseCaseFixture.CriarAutenticacaoUseCaseDtoAdmin();

        // Assert
        dto1.Should().NotBeSameAs(dto2, "devem ser instâncias diferentes");
        dto1.Email.Should().NotBe(dto2.Email, "devem ter emails diferentes");
        dto1.Senha.Should().NotBe(dto2.Senha, "devem ter senhas diferentes");
    }
}
