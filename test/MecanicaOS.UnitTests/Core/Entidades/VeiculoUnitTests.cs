using Core.Entidades;
using Core.Entidades.Abstratos;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class VeiculoUnitTests
{
    [Fact]
    public void Veiculo_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var veiculo = new Veiculo();

        // Assert
        veiculo.Should().BeAssignableTo<Entidade>("Veiculo deve herdar de Entidade");
        veiculo.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        veiculo.Ativo.Should().BeTrue("Veiculo deve estar ativo por padrão");
        veiculo.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        veiculo.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void Veiculo_QuandoDefinidaPlaca_DeveArmazenarCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var placaEsperada = "ABC-1234";

        // Act
        veiculo.Placa = placaEsperada;

        // Assert
        veiculo.Placa.Should().Be(placaEsperada, "a placa deve ser armazenada corretamente");
    }

    [Fact]
    public void Veiculo_QuandoDefinidaMarca_DeveArmazenarCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var marcaEsperada = "Toyota";

        // Act
        veiculo.Marca = marcaEsperada;

        // Assert
        veiculo.Marca.Should().Be(marcaEsperada, "a marca deve ser armazenada corretamente");
    }

    [Fact]
    public void Veiculo_QuandoDefinidoModelo_DeveArmazenarCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var modeloEsperado = "Corolla";

        // Act
        veiculo.Modelo = modeloEsperado;

        // Assert
        veiculo.Modelo.Should().Be(modeloEsperado, "o modelo deve ser armazenado corretamente");
    }

    [Fact]
    public void Veiculo_QuandoDefinidoAno_DeveArmazenarCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var anoEsperado = "2020";

        // Act
        veiculo.Ano = anoEsperado;

        // Assert
        veiculo.Ano.Should().Be(anoEsperado, "o ano deve ser armazenado corretamente");
    }

    [Fact]
    public void Veiculo_QuandoDefinidaCor_DeveArmazenarCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var corEsperada = "Branco";

        // Act
        veiculo.Cor = corEsperada;

        // Assert
        veiculo.Cor.Should().Be(corEsperada, "a cor deve ser armazenada corretamente");
    }

    [Fact]
    public void Veiculo_QuandoDefinidoClienteId_DeveArmazenarCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var clienteIdEsperado = Guid.NewGuid();

        // Act
        veiculo.ClienteId = clienteIdEsperado;

        // Assert
        veiculo.ClienteId.Should().Be(clienteIdEsperado, "o ClienteId deve ser armazenado corretamente");
    }

    [Fact]
    public void Veiculo_QuandoDefinidaAnotacoes_DeveArmazenarCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var anotacoesEsperadas = "Veículo em bom estado";

        // Act
        veiculo.Anotacoes = anotacoesEsperadas;

        // Assert
        veiculo.Anotacoes.Should().Be(anotacoesEsperadas, "as anotações devem ser armazenadas corretamente");
    }

    [Fact]
    public void Veiculo_QuandoDesativado_DeveMarcarComoInativo()
    {
        // Arrange
        var veiculo = new Veiculo { Ativo = true };

        // Act
        veiculo.Desativar();

        // Assert
        veiculo.Ativo.Should().BeFalse("o veículo deve estar marcado como inativo");
    }

    [Fact]
    public void Veiculo_QuandoAtivado_DeveMarcarComoAtivo()
    {
        // Arrange
        var veiculo = new Veiculo { Ativo = false };

        // Act
        veiculo.Ativar();

        // Assert
        veiculo.Ativo.Should().BeTrue("o veículo deve estar marcado como ativo");
    }

    [Fact]
    public void Veiculo_QuandoComparadoComOutroVeiculoComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var veiculo1 = new Veiculo { Id = id, Placa = "ABC-1234" };
        var veiculo2 = new Veiculo { Id = id, Placa = "XYZ-5678" };

        // Act & Assert
        veiculo1.Should().Be(veiculo2, "veículos com mesmo Id devem ser considerados iguais");
        veiculo1.GetHashCode().Should().Be(veiculo2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }

    [Fact]
    public void Veiculo_QuandoComparadoComVeiculoComIdDiferente_NaoDeveSerIgual()
    {
        // Arrange
        var veiculo1 = new Veiculo { Id = Guid.NewGuid(), Placa = "ABC-1234" };
        var veiculo2 = new Veiculo { Id = Guid.NewGuid(), Placa = "ABC-1234" };

        // Act & Assert
        veiculo1.Should().NotBe(veiculo2, "veículos com Ids diferentes não devem ser iguais");
    }
}
