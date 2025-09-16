using Xunit;
using FluentAssertions;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.Autenticacao;

namespace MecanicaOS.UnitTests.Core.DTOs.Entidades;

public class EstoqueEntityDtoUnitTests
{
    [Fact]
    public void EstoqueEntityDto_QuandoCriado_DeveHerdarDeEntityDto()
    {
        // Arrange & Act
        var dto = new EstoqueEntityDto();

        // Assert
        dto.Should().BeAssignableTo<EntityDto>("EstoqueEntityDto deve herdar de EntityDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void EstoqueEntityDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new EstoqueEntityDto();
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
    public void EstoqueEntityDto_QuandoDefinidoInsumo_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EstoqueEntityDto();
        var insumoEsperado = "Óleo Motor 5W30";

        // Act
        dto.Insumo = insumoEsperado;

        // Assert
        dto.Insumo.Should().Be(insumoEsperado, "o insumo deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void EstoqueEntityDto_QuandoDefinidaDescricao_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EstoqueEntityDto();
        var descricaoEsperada = "Óleo sintético para motores";

        // Act
        dto.Descricao = descricaoEsperada;

        // Assert
        dto.Descricao.Should().Be(descricaoEsperada, "a descrição deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void EstoqueEntityDto_QuandoDefinidoPreco_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EstoqueEntityDto();
        var precoEsperado = 45.90m;

        // Act
        dto.Preco = precoEsperado;

        // Assert
        dto.Preco.Should().Be(precoEsperado, "o preço deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void EstoqueEntityDto_QuandoDefinidaQuantidadeDisponivel_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EstoqueEntityDto();
        var quantidadeEsperada = 50;

        // Act
        dto.QuantidadeDisponivel = quantidadeEsperada;

        // Assert
        dto.QuantidadeDisponivel.Should().Be(quantidadeEsperada, "a quantidade disponível deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void EstoqueEntityDto_QuandoDefinidaQuantidadeMinima_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EstoqueEntityDto();
        var quantidadeMinimaEsperada = 10;

        // Act
        dto.QuantidadeMinima = quantidadeMinimaEsperada;

        // Assert
        dto.QuantidadeMinima.Should().Be(quantidadeMinimaEsperada, "a quantidade mínima deve ser armazenada corretamente no DTO");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public void EstoqueEntityDto_QuandoDefinidaQuantidadeDisponivel_DeveAceitarValoresPositivos(int quantidade)
    {
        // Arrange
        var dto = new EstoqueEntityDto();

        // Act
        dto.QuantidadeDisponivel = quantidade;

        // Assert
        dto.QuantidadeDisponivel.Should().Be(quantidade, "deve aceitar qualquer quantidade positiva ou zero no DTO");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(20)]
    public void EstoqueEntityDto_QuandoDefinidaQuantidadeMinima_DeveAceitarValoresPositivos(int quantidadeMinima)
    {
        // Arrange
        var dto = new EstoqueEntityDto();

        // Act
        dto.QuantidadeMinima = quantidadeMinima;

        // Assert
        dto.QuantidadeMinima.Should().Be(quantidadeMinima, "deve aceitar qualquer quantidade mínima positiva ou zero no DTO");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(10.50)]
    [InlineData(999.99)]
    public void EstoqueEntityDto_QuandoDefinidoPreco_DeveAceitarValoresPositivos(decimal preco)
    {
        // Arrange
        var dto = new EstoqueEntityDto();

        // Act
        dto.Preco = preco;

        // Assert
        dto.Preco.Should().Be(preco, "deve aceitar qualquer preço positivo no DTO");
    }

    [Fact]
    public void EstoqueEntityDto_QuandoDescricaoNula_DevePermitirValorNulo()
    {
        // Arrange
        var dto = new EstoqueEntityDto();

        // Act
        dto.Descricao = null;

        // Assert
        dto.Descricao.Should().BeNull("a descrição pode ser nula no DTO");
    }

    [Theory]
    [InlineData("Óleo Motor 5W30")]
    [InlineData("Filtro de Ar")]
    [InlineData("Pastilha de Freio")]
    public void EstoqueEntityDto_QuandoDefinidoInsumoComValoresDiferentes_DeveArmazenarCorretamente(string insumo)
    {
        // Arrange
        var dto = new EstoqueEntityDto();

        // Act
        dto.Insumo = insumo;

        // Assert
        dto.Insumo.Should().Be(insumo, "o insumo deve ser armazenado independente do conteúdo no DTO");
    }

    [Theory]
    [InlineData("Óleo sintético de alta qualidade")]
    [InlineData("Filtro original do fabricante")]
    [InlineData("Pastilha cerâmica premium")]
    public void EstoqueEntityDto_QuandoDefinidaDescricaoComValoresDiferentes_DeveArmazenarCorretamente(string descricao)
    {
        // Arrange
        var dto = new EstoqueEntityDto();

        // Act
        dto.Descricao = descricao;

        // Assert
        dto.Descricao.Should().Be(descricao, "a descrição deve ser armazenada independente do conteúdo no DTO");
    }
}
