using Xunit;
using FluentAssertions;
using Core.Entidades;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class ClienteUnitTests
{
    [Fact]
    public void Cliente_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var cliente = new Cliente();

        // Assert
        cliente.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        cliente.Ativo.Should().BeTrue("Cliente deve estar ativo por padrão");
        cliente.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        cliente.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void Cliente_QuandoDefinidoNome_DeveArmazenarCorretamente()
    {
        // Arrange
        var cliente = new Cliente();
        var nomeEsperado = "João Silva";

        // Act
        cliente.Nome = nomeEsperado;

        // Assert
        cliente.Nome.Should().Be(nomeEsperado, "o nome deve ser armazenado corretamente");
    }

    [Fact]
    public void Cliente_QuandoDefinidoDocumento_DeveArmazenarCorretamente()
    {
        // Arrange
        var cliente = new Cliente();
        var documentoEsperado = "12345678901";

        // Act
        cliente.Documento = documentoEsperado;

        // Assert
        cliente.Documento.Should().Be(documentoEsperado, "o documento deve ser armazenado corretamente");
    }

    [Theory]
    [InlineData(TipoCliente.PessoaFisica)]
    [InlineData(TipoCliente.PessoaJuridico)]
    public void Cliente_QuandoDefinidoTipoCliente_DeveArmazenarCorretamente(TipoCliente tipo)
    {
        // Arrange
        var cliente = new Cliente();

        // Act
        cliente.TipoCliente = tipo;

        // Assert
        cliente.TipoCliente.Should().Be(tipo, "o tipo de cliente deve ser armazenado corretamente");
    }

    [Fact]
    public void Cliente_QuandoDesativado_DeveMarcarComoInativo()
    {
        // Arrange
        var cliente = new Cliente { Ativo = true };

        // Act
        cliente.Desativar();

        // Assert
        cliente.Ativo.Should().BeFalse("o cliente deve estar marcado como inativo");
    }

    [Fact]
    public void Cliente_QuandoAtivado_DeveMarcarComoAtivo()
    {
        // Arrange
        var cliente = new Cliente { Ativo = false };

        // Act
        cliente.Ativar();

        // Assert
        cliente.Ativo.Should().BeTrue("o cliente deve estar marcado como ativo");
    }

    [Fact]
    public void Cliente_QuandoComparadoComOutroClienteComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cliente1 = new Cliente { Id = id, Nome = "João" };
        var cliente2 = new Cliente { Id = id, Nome = "Maria" };

        // Act & Assert
        cliente1.Should().Be(cliente2, "clientes com mesmo Id devem ser considerados iguais");
        cliente1.GetHashCode().Should().Be(cliente2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }

    [Fact]
    public void Cliente_QuandoComparadoComClienteComIdDiferente_NaoDeveSerIgual()
    {
        // Arrange
        var cliente1 = new Cliente { Id = Guid.NewGuid(), Nome = "João" };
        var cliente2 = new Cliente { Id = Guid.NewGuid(), Nome = "João" };

        // Act & Assert
        cliente1.Should().NotBe(cliente2, "clientes com Ids diferentes não devem ser iguais");
    }
}
