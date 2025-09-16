using Core.Entidades;
using Core.Entidades.Abstratos;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class AlertaEstoqueUnitTests
{
    [Fact]
    public void AlertaEstoque_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var alertaEstoque = new AlertaEstoque();

        // Assert
        alertaEstoque.Should().BeAssignableTo<Entidade>("AlertaEstoque deve herdar de Entidade");
        alertaEstoque.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        alertaEstoque.Ativo.Should().BeTrue("AlertaEstoque deve estar ativo por padrão");
        alertaEstoque.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        alertaEstoque.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void AlertaEstoque_QuandoDefinidoEstoqueId_DeveArmazenarCorretamente()
    {
        // Arrange
        var alertaEstoque = new AlertaEstoque();
        var estoqueIdEsperado = Guid.NewGuid();

        // Act
        alertaEstoque.EstoqueId = estoqueIdEsperado;

        // Assert
        alertaEstoque.EstoqueId.Should().Be(estoqueIdEsperado, "o EstoqueId deve ser armazenado corretamente");
    }

    [Fact]
    public void AlertaEstoque_QuandoDefinidoEstoque_DeveArmazenarReferencia()
    {
        // Arrange
        var alertaEstoque = new AlertaEstoque();
        var estoque = new Estoque { Insumo = "Óleo Motor" };

        // Act
        alertaEstoque.Estoque = estoque;

        // Assert
        alertaEstoque.Estoque.Should().Be(estoque, "a referência do estoque deve ser armazenada corretamente");
        alertaEstoque.Estoque.Insumo.Should().Be("Óleo Motor", "as propriedades do estoque devem ser preservadas");
    }

    [Fact]
    public void AlertaEstoque_QuandoComparadoComOutroAlertaComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var alertaEstoque1 = new AlertaEstoque { Id = id, EstoqueId = Guid.NewGuid() };
        var alertaEstoque2 = new AlertaEstoque { Id = id, EstoqueId = Guid.NewGuid() };

        // Act & Assert
        alertaEstoque1.Should().Be(alertaEstoque2, "alertas de estoque com mesmo Id devem ser considerados iguais");
        alertaEstoque1.GetHashCode().Should().Be(alertaEstoque2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }

    [Fact]
    public void AlertaEstoque_QuandoComparadoComAlertaComIdDiferente_NaoDeveSerIgual()
    {
        // Arrange
        var alertaEstoque1 = new AlertaEstoque { Id = Guid.NewGuid(), EstoqueId = Guid.NewGuid() };
        var alertaEstoque2 = new AlertaEstoque { Id = Guid.NewGuid(), EstoqueId = alertaEstoque1.EstoqueId };

        // Act & Assert
        alertaEstoque1.Should().NotBe(alertaEstoque2, "alertas de estoque com Ids diferentes não devem ser iguais");
    }

    [Fact]
    public void AlertaEstoque_QuandoDesativado_DeveMarcarComoInativo()
    {
        // Arrange
        var alertaEstoque = new AlertaEstoque { Ativo = true };

        // Act
        alertaEstoque.Desativar();

        // Assert
        alertaEstoque.Ativo.Should().BeFalse("o alerta de estoque deve estar marcado como inativo");
    }

    [Fact]
    public void AlertaEstoque_QuandoAtivado_DeveMarcarComoAtivo()
    {
        // Arrange
        var alertaEstoque = new AlertaEstoque { Ativo = false };

        // Act
        alertaEstoque.Ativar();

        // Assert
        alertaEstoque.Ativo.Should().BeTrue("o alerta de estoque deve estar marcado como ativo");
    }

    [Fact]
    public void AlertaEstoque_QuandoDefinidoEstoqueIdVazio_DeveArmazenarCorretamente()
    {
        // Arrange
        var alertaEstoque = new AlertaEstoque();
        var estoqueIdVazio = Guid.Empty;

        // Act
        alertaEstoque.EstoqueId = estoqueIdVazio;

        // Assert
        alertaEstoque.EstoqueId.Should().Be(estoqueIdVazio, "deve aceitar EstoqueId vazio");
    }
}
