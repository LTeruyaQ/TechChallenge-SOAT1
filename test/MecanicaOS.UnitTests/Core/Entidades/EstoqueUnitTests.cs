using Xunit;
using FluentAssertions;
using Core.Entidades;
using Core.Entidades.Abstratos;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class EstoqueUnitTests
{
    [Fact]
    public void Estoque_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var estoque = new Estoque();

        // Assert
        estoque.Should().BeAssignableTo<Entidade>("Estoque deve herdar de Entidade");
        estoque.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        estoque.Ativo.Should().BeTrue("Estoque deve estar ativo por padrão");
        estoque.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        estoque.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void Estoque_QuandoDefinidoInsumo_DeveArmazenarCorretamente()
    {
        // Arrange
        var estoque = new Estoque();
        var insumoEsperado = "Óleo Motor 5W30";

        // Act
        estoque.Insumo = insumoEsperado;

        // Assert
        estoque.Insumo.Should().Be(insumoEsperado, "o insumo deve ser armazenado corretamente");
    }

    [Fact]
    public void Estoque_QuandoDefinidaDescricao_DeveArmazenarCorretamente()
    {
        // Arrange
        var estoque = new Estoque();
        var descricaoEsperada = "Óleo sintético para motores";

        // Act
        estoque.Descricao = descricaoEsperada;

        // Assert
        estoque.Descricao.Should().Be(descricaoEsperada, "a descrição deve ser armazenada corretamente");
    }

    [Fact]
    public void Estoque_QuandoDefinidoPreco_DeveArmazenarCorretamente()
    {
        // Arrange
        var estoque = new Estoque();
        var precoEsperado = 45.90m;

        // Act
        estoque.Preco = precoEsperado;

        // Assert
        estoque.Preco.Should().Be(precoEsperado, "o preço deve ser armazenado corretamente");
    }

    [Fact]
    public void Estoque_QuandoDefinidaQuantidadeDisponivel_DeveArmazenarCorretamente()
    {
        // Arrange
        var estoque = new Estoque();
        var quantidadeEsperada = 50;

        // Act
        estoque.QuantidadeDisponivel = quantidadeEsperada;

        // Assert
        estoque.QuantidadeDisponivel.Should().Be(quantidadeEsperada, "a quantidade disponível deve ser armazenada corretamente");
    }

    [Fact]
    public void Estoque_QuandoDefinidaQuantidadeMinima_DeveArmazenarCorretamente()
    {
        // Arrange
        var estoque = new Estoque();
        var quantidadeMinimaEsperada = 10;

        // Act
        estoque.QuantidadeMinima = quantidadeMinimaEsperada;

        // Assert
        estoque.QuantidadeMinima.Should().Be(quantidadeMinimaEsperada, "a quantidade mínima deve ser armazenada corretamente");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Estoque_QuandoDefinidaQuantidadeDisponivel_DeveAceitarValoresPositivos(int quantidade)
    {
        // Arrange
        var estoque = new Estoque();

        // Act
        estoque.QuantidadeDisponivel = quantidade;

        // Assert
        estoque.QuantidadeDisponivel.Should().Be(quantidade, "deve aceitar qualquer quantidade positiva ou zero");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(20)]
    public void Estoque_QuandoDefinidaQuantidadeMinima_DeveAceitarValoresPositivos(int quantidadeMinima)
    {
        // Arrange
        var estoque = new Estoque();

        // Act
        estoque.QuantidadeMinima = quantidadeMinima;

        // Assert
        estoque.QuantidadeMinima.Should().Be(quantidadeMinima, "deve aceitar qualquer quantidade mínima positiva ou zero");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(10.50)]
    [InlineData(999.99)]
    public void Estoque_QuandoDefinidoPreco_DeveAceitarValoresPositivos(decimal preco)
    {
        // Arrange
        var estoque = new Estoque();

        // Act
        estoque.Preco = preco;

        // Assert
        estoque.Preco.Should().Be(preco, "deve aceitar qualquer preço positivo");
    }

    [Fact]
    public void Estoque_QuandoDescricaoNula_DevePermitirValorNulo()
    {
        // Arrange
        var estoque = new Estoque();

        // Act
        estoque.Descricao = null;

        // Assert
        estoque.Descricao.Should().BeNull("a descrição pode ser nula");
    }

    [Fact]
    public void Estoque_QuandoComparadoComOutroEstoqueComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var estoque1 = new Estoque { Id = id, Insumo = "Insumo 1" };
        var estoque2 = new Estoque { Id = id, Insumo = "Insumo 2" };

        // Act & Assert
        estoque1.Should().Be(estoque2, "estoques com mesmo Id devem ser considerados iguais");
        estoque1.GetHashCode().Should().Be(estoque2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }

    [Fact]
    public void Estoque_QuandoComparadoComEstoqueComIdDiferente_NaoDeveSerIgual()
    {
        // Arrange
        var estoque1 = new Estoque { Id = Guid.NewGuid(), Insumo = "Insumo" };
        var estoque2 = new Estoque { Id = Guid.NewGuid(), Insumo = "Insumo" };

        // Act & Assert
        estoque1.Should().NotBe(estoque2, "estoques com Ids diferentes não devem ser iguais");
    }

    [Fact]
    public void Estoque_QuandoDesativado_DeveMarcarComoInativo()
    {
        // Arrange
        var estoque = new Estoque { Ativo = true };

        // Act
        estoque.Desativar();

        // Assert
        estoque.Ativo.Should().BeFalse("o estoque deve estar marcado como inativo");
    }

    [Fact]
    public void Estoque_QuandoAtivado_DeveMarcarComoAtivo()
    {
        // Arrange
        var estoque = new Estoque { Ativo = false };

        // Act
        estoque.Ativar();

        // Assert
        estoque.Ativo.Should().BeTrue("o estoque deve estar marcado como ativo");
    }
}
