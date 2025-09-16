using Core.Entidades;
using Core.Entidades.Abstratos;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class ContatoUnitTests
{
    [Fact]
    public void Contato_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var contato = new Contato();

        // Assert
        contato.Should().BeAssignableTo<Entidade>("Contato deve herdar de Entidade");
        contato.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        contato.Ativo.Should().BeTrue("Contato deve estar ativo por padrão");
        contato.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        contato.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void Contato_QuandoDefinidoTelefone_DeveArmazenarCorretamente()
    {
        // Arrange
        var contato = new Contato();
        var telefoneEsperado = "(11) 99999-9999";

        // Act
        contato.Telefone = telefoneEsperado;

        // Assert
        contato.Telefone.Should().Be(telefoneEsperado, "o telefone deve ser armazenado corretamente");
    }

    [Fact]
    public void Contato_QuandoDefinidoEmail_DeveArmazenarCorretamente()
    {
        // Arrange
        var contato = new Contato();
        var emailEsperado = "cliente@exemplo.com";

        // Act
        contato.Email = emailEsperado;

        // Assert
        contato.Email.Should().Be(emailEsperado, "o email deve ser armazenado corretamente");
    }

    [Fact]
    public void Contato_QuandoDefinidoIdCliente_DeveArmazenarCorretamente()
    {
        // Arrange
        var contato = new Contato();
        var idClienteEsperado = Guid.NewGuid();

        // Act
        contato.IdCliente = idClienteEsperado;

        // Assert
        contato.IdCliente.Should().Be(idClienteEsperado, "o IdCliente deve ser armazenado corretamente");
    }

    [Theory]
    [InlineData("(11) 99999-9999")]
    [InlineData("11999999999")]
    [InlineData("+55 11 99999-9999")]
    public void Contato_QuandoDefinidoTelefoneComFormatosDiferentes_DeveArmazenarCorretamente(string telefone)
    {
        // Arrange
        var contato = new Contato();

        // Act
        contato.Telefone = telefone;

        // Assert
        contato.Telefone.Should().Be(telefone, "o telefone deve ser armazenado independente do formato");
    }

    [Theory]
    [InlineData("usuario@dominio.com")]
    [InlineData("teste.email@empresa.com.br")]
    [InlineData("contato+tag@exemplo.org")]
    public void Contato_QuandoDefinidoEmailComFormatosDiferentes_DeveArmazenarCorretamente(string email)
    {
        // Arrange
        var contato = new Contato();

        // Act
        contato.Email = email;

        // Assert
        contato.Email.Should().Be(email, "o email deve ser armazenado independente do formato");
    }

    [Fact]
    public void Contato_QuandoDesativado_DeveMarcarComoInativo()
    {
        // Arrange
        var contato = new Contato { Ativo = true };

        // Act
        contato.Desativar();

        // Assert
        contato.Ativo.Should().BeFalse("o contato deve estar marcado como inativo");
    }

    [Fact]
    public void Contato_QuandoAtivado_DeveMarcarComoAtivo()
    {
        // Arrange
        var contato = new Contato { Ativo = false };

        // Act
        contato.Ativar();

        // Assert
        contato.Ativo.Should().BeTrue("o contato deve estar marcado como ativo");
    }

    [Fact]
    public void Contato_QuandoComparadoComOutroContatoComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var contato1 = new Contato { Id = id, Email = "email1@teste.com" };
        var contato2 = new Contato { Id = id, Email = "email2@teste.com" };

        // Act & Assert
        contato1.Should().Be(contato2, "contatos com mesmo Id devem ser considerados iguais");
        contato1.GetHashCode().Should().Be(contato2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }

    [Fact]
    public void Contato_QuandoComparadoComContatoComIdDiferente_NaoDeveSerIgual()
    {
        // Arrange
        var contato1 = new Contato { Id = Guid.NewGuid(), Email = "email@teste.com" };
        var contato2 = new Contato { Id = Guid.NewGuid(), Email = "email@teste.com" };

        // Act & Assert
        contato1.Should().NotBe(contato2, "contatos com Ids diferentes não devem ser iguais");
    }
}
