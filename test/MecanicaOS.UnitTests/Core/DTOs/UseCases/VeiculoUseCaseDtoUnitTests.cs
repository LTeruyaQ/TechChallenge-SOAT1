using Core.DTOs.UseCases.Veiculo;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using Xunit;

namespace MecanicaOS.UnitTests.Core.DTOs.UseCases;

public class VeiculoUseCaseDtoUnitTests
{
    [Fact]
    public void CadastrarVeiculoUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarCadastrarVeiculoUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Placa.Should().Be("ABC-1234", "a placa deve ser armazenada corretamente");
        dto.Marca.Should().Be("Toyota", "a marca deve ser armazenada corretamente");
        dto.Modelo.Should().Be("Corolla", "o modelo deve ser armazenado corretamente");
        dto.Cor.Should().Be("Prata", "a cor deve ser armazenada corretamente");
        dto.Ano.Should().Be("2020", "o ano deve ser armazenado corretamente");
        dto.Anotacoes.Should().Be("Veículo em bom estado geral", "as anotações devem ser armazenadas corretamente");
        dto.ClienteId.Should().NotBeEmpty("deve ter um ClienteId válido");
    }

    [Fact]
    public void CadastrarVeiculoUseCaseDto_DevePermitirAnotacoesNulas()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarCadastrarVeiculoUseCaseDtoSemAnotacoes();

        // Assert
        dto.Anotacoes.Should().BeNull("deve permitir anotações nulas");
        dto.Placa.Should().Be("XYZ-9876", "deve manter placa obrigatória");
        dto.Marca.Should().Be("Honda", "deve manter marca obrigatória");
        dto.Modelo.Should().Be("Civic", "deve manter modelo obrigatório");
        dto.ClienteId.Should().NotBeEmpty("deve manter ClienteId válido");
    }

    [Fact]
    public void CadastrarVeiculoUseCaseDto_DevePermitirVeiculoAntigo()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarCadastrarVeiculoUseCaseDtoVeiculoAntigo();

        // Assert
        dto.Ano.Should().Be("1990", "deve aceitar anos antigos");
        dto.Marca.Should().Be("Volkswagen", "deve aceitar marcas clássicas");
        dto.Modelo.Should().Be("Fusca", "deve aceitar modelos clássicos");
        dto.Anotacoes.Should().Contain("clássico", "deve ter anotações sobre veículo clássico");
        dto.Anotacoes.Should().Contain("cuidados especiais", "deve mencionar cuidados especiais");
    }

    [Fact]
    public void CadastrarVeiculoUseCaseDto_DevePermitirVeiculoNovo()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarCadastrarVeiculoUseCaseDtoVeiculoNovo();

        // Assert
        dto.Ano.Should().Be("2024", "deve aceitar anos recentes");
        dto.Marca.Should().Be("BMW", "deve aceitar marcas premium");
        dto.Modelo.Should().Be("X3", "deve aceitar modelos premium");
        dto.Anotacoes.Should().Contain("zero quilômetro", "deve mencionar veículo novo");
        dto.Anotacoes.Should().Contain("garantia", "deve mencionar garantia");
    }

    [Theory]
    [InlineData("ABC-1234")]
    [InlineData("XYZ-9876")]
    [InlineData("OLD-1990")]
    [InlineData("NEW-2024")]
    public void CadastrarVeiculoUseCaseDto_DeveAceitarDiferentesPlacas(string placa)
    {
        // Arrange & Act
        var dto = new CadastrarVeiculoUseCaseDto
        {
            Placa = placa,
            Marca = "Teste",
            Modelo = "Teste",
            Cor = "Teste",
            Ano = "2020",
            ClienteId = Guid.NewGuid()
        };

        // Assert
        dto.Placa.Should().Be(placa, "deve aceitar a placa especificada");
    }

    [Theory]
    [InlineData("Toyota", "Corolla")]
    [InlineData("Honda", "Civic")]
    [InlineData("Volkswagen", "Fusca")]
    [InlineData("BMW", "X3")]
    public void CadastrarVeiculoUseCaseDto_DeveAceitarDiferentesMarcasModelos(string marca, string modelo)
    {
        // Arrange & Act
        var dto = new CadastrarVeiculoUseCaseDto
        {
            Placa = "TST-1234",
            Marca = marca,
            Modelo = modelo,
            Cor = "Branco",
            Ano = "2020",
            ClienteId = Guid.NewGuid()
        };

        // Assert
        dto.Marca.Should().Be(marca, "deve aceitar a marca especificada");
        dto.Modelo.Should().Be(modelo, "deve aceitar o modelo especificado");
    }

    [Fact]
    public void AtualizarVeiculoUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarAtualizarVeiculoUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Placa.Should().Be("UPD-5678", "a placa deve ser armazenada corretamente");
        dto.Marca.Should().Be("Ford", "a marca deve ser armazenada corretamente");
        dto.Modelo.Should().Be("Focus", "o modelo deve ser armazenado corretamente");
        dto.Cor.Should().Be("Vermelho", "a cor deve ser armazenada corretamente");
        dto.Ano.Should().Be("2021", "o ano deve ser armazenado corretamente");
        dto.Anotacoes.Should().Be("Anotações atualizadas do veículo", "as anotações devem ser armazenadas corretamente");
        dto.ClienteId.Should().NotBeEmpty("deve ter um ClienteId válido");
    }

    [Fact]
    public void AtualizarVeiculoUseCaseDto_DevePermitirCamposNulos()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarAtualizarVeiculoUseCaseDtoComCamposNulos();

        // Assert
        dto.Placa.Should().BeNull("deve permitir placa nula");
        dto.Marca.Should().BeNull("deve permitir marca nula");
        dto.Modelo.Should().BeNull("deve permitir modelo nulo");
        dto.Cor.Should().BeNull("deve permitir cor nula");
        dto.Ano.Should().BeNull("deve permitir ano nulo");
        dto.Anotacoes.Should().BeNull("deve permitir anotações nulas");
        dto.ClienteId.Should().BeNull("deve permitir ClienteId nulo");
    }

    [Fact]
    public void AtualizarVeiculoUseCaseDto_DevePermitirAtualizacaoParcialCor()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarAtualizarVeiculoUseCaseDtoApenasCor();

        // Assert
        dto.Cor.Should().Be("Verde", "deve permitir atualização apenas da cor");
        dto.Placa.Should().BeNull("outros campos podem permanecer nulos");
        dto.Marca.Should().BeNull("outros campos podem permanecer nulos");
        dto.ClienteId.Should().BeNull("outros campos podem permanecer nulos");
    }

    [Fact]
    public void AtualizarVeiculoUseCaseDto_DevePermitirAtualizacaoParcialAnotacoes()
    {
        // Arrange & Act
        var dto = VeiculoUseCaseFixture.CriarAtualizarVeiculoUseCaseDtoApenasAnotacoes();

        // Assert
        dto.Anotacoes.Should().Be("Novas anotações importantes sobre o veículo", 
            "deve permitir atualização apenas das anotações");
        dto.Placa.Should().BeNull("outros campos podem permanecer nulos");
        dto.Cor.Should().BeNull("outros campos podem permanecer nulos");
    }

    [Fact]
    public void CadastrarVeiculoUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = VeiculoUseCaseFixture.CriarListaCadastrarVeiculoUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(4, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Placa), 
            "todos os DTOs devem ter placa");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Marca), 
            "todos os DTOs devem ter marca");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Modelo), 
            "todos os DTOs devem ter modelo");
        lista.Should().OnlyContain(dto => dto.ClienteId != Guid.Empty, 
            "todos os DTOs devem ter ClienteId válido");
    }

    [Fact]
    public void AtualizarVeiculoUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = VeiculoUseCaseFixture.CriarListaAtualizarVeiculoUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(4, "deve conter todos os DTOs da fixture");
    }

    [Fact]
    public void VeiculoUseCaseDto_DevePermitirAlteracaoDePropriedades()
    {
        // Arrange
        var dto = VeiculoUseCaseFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        var novaPlaca = "NEW-9999";
        var novaCor = "Azul";

        // Act
        dto.Placa = novaPlaca;
        dto.Cor = novaCor;

        // Assert
        dto.Placa.Should().Be(novaPlaca, "deve permitir alteração da placa");
        dto.Cor.Should().Be(novaCor, "deve permitir alteração da cor");
    }

    [Fact]
    public void VeiculoUseCaseDto_DeveSerDistintoEntreInstancias()
    {
        // Arrange & Act
        var dto1 = VeiculoUseCaseFixture.CriarCadastrarVeiculoUseCaseDtoValido();
        var dto2 = VeiculoUseCaseFixture.CriarCadastrarVeiculoUseCaseDtoVeiculoAntigo();

        // Assert
        dto1.Should().NotBeSameAs(dto2, "devem ser instâncias diferentes");
        dto1.Placa.Should().NotBe(dto2.Placa, "devem ter placas diferentes");
        dto1.Marca.Should().NotBe(dto2.Marca, "devem ter marcas diferentes");
        dto1.Modelo.Should().NotBe(dto2.Modelo, "devem ter modelos diferentes");
        dto1.Ano.Should().NotBe(dto2.Ano, "devem ter anos diferentes");
        dto1.ClienteId.Should().NotBe(dto2.ClienteId, "devem ter ClienteIds diferentes");
    }
}
