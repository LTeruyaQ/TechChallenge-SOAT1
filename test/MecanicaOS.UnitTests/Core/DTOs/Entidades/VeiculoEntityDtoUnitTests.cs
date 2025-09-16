using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Veiculo;

namespace MecanicaOS.UnitTests.Core.DTOs.Entidades;

public class VeiculoEntityDtoUnitTests
{
    [Fact]
    public void VeiculoEntityDto_QuandoCriado_DeveHerdarDeEntityDto()
    {
        // Arrange & Act
        var dto = new VeiculoEntityDto();

        // Assert
        dto.Should().BeAssignableTo<EntityDto>("VeiculoEntityDto deve herdar de EntityDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var id = Guid.NewGuid();
        var dataCadastro = DateTime.Now;
        var dataAtualizacao = DateTime.Now.AddMinutes(5);

        // Act
        dto.Id = id;
        dto.DataCadastro = dataCadastro;
        dto.DataAtualizacao = dataAtualizacao;
        dto.Ativo = true;

        // Assert
        dto.Id.Should().Be(id, "o ID deve ser preservado corretamente");
        dto.DataCadastro.Should().Be(dataCadastro, "a data de cadastro deve ser preservada");
        dto.DataAtualizacao.Should().Be(dataAtualizacao, "a data de atualização deve ser preservada");
        dto.Ativo.Should().BeTrue("o status ativo deve ser preservado");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidaPlaca_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var placaEsperada = "ABC-1234";

        // Act
        dto.Placa = placaEsperada;

        // Assert
        dto.Placa.Should().Be(placaEsperada, "a placa deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidaMarca_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var marcaEsperada = "Toyota";

        // Act
        dto.Marca = marcaEsperada;

        // Assert
        dto.Marca.Should().Be(marcaEsperada, "a marca deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidoModelo_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var modeloEsperado = "Corolla";

        // Act
        dto.Modelo = modeloEsperado;

        // Assert
        dto.Modelo.Should().Be(modeloEsperado, "o modelo deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidoAno_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var anoEsperado = "2020";

        // Act
        dto.Ano = anoEsperado;

        // Assert
        dto.Ano.Should().Be(anoEsperado, "o ano deve ser armazenado corretamente no DTO como string");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidaCor_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var corEsperada = "Branco";

        // Act
        dto.Cor = corEsperada;

        // Assert
        dto.Cor.Should().Be(corEsperada, "a cor deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidoClienteId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var clienteIdEsperado = Guid.NewGuid();

        // Act
        dto.ClienteId = clienteIdEsperado;

        // Assert
        dto.ClienteId.Should().Be(clienteIdEsperado, "o ClienteId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void VeiculoEntityDto_QuandoDefinidaAnotacoes_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new VeiculoEntityDto();
        var anotacoesEsperadas = "Veículo em bom estado";

        // Act
        dto.Anotacoes = anotacoesEsperadas;

        // Assert
        dto.Anotacoes.Should().Be(anotacoesEsperadas, "as anotações devem ser armazenadas corretamente no DTO");
    }

    [Theory]
    [InlineData("2020")]
    [InlineData("2021")]
    [InlineData("2022")]
    public void VeiculoEntityDto_QuandoDefinidoAnoComoString_DeveArmazenarCorretamente(string ano)
    {
        // Arrange
        var dto = new VeiculoEntityDto();

        // Act
        dto.Ano = ano;

        // Assert
        dto.Ano.Should().Be(ano, "o ano deve ser armazenado como string no DTO");
    }

    [Theory]
    [InlineData("ABC-1234")]
    [InlineData("XYZ-5678")]
    [InlineData("BRA2E19")]
    public void VeiculoEntityDto_QuandoDefinidaPlacaComFormatosDiferentes_DeveArmazenarCorretamente(string placa)
    {
        // Arrange
        var dto = new VeiculoEntityDto();

        // Act
        dto.Placa = placa;

        // Assert
        dto.Placa.Should().Be(placa, "a placa deve ser armazenada independente do formato");
    }
}
