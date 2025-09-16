using Core.Entidades;
using Core.Entidades.Abstratos;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class InsumoOSUnitTests
{
    [Fact]
    public void InsumoOS_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var insumoOS = new InsumoOS();

        // Assert
        insumoOS.Should().BeAssignableTo<Entidade>("InsumoOS deve herdar de Entidade");
        insumoOS.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        insumoOS.Ativo.Should().BeTrue("InsumoOS deve estar ativo por padrão");
        insumoOS.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        insumoOS.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void InsumoOS_QuandoDefinidoOrdemServicoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var insumoOS = new InsumoOS();
        var ordemServicoIdEsperado = Guid.NewGuid();

        // Act
        insumoOS.OrdemServicoId = ordemServicoIdEsperado;

        // Assert
        insumoOS.OrdemServicoId.Should().Be(ordemServicoIdEsperado, "o OrdemServicoId deve ser armazenado corretamente");
    }

    [Fact]
    public void InsumoOS_QuandoDefinidoEstoqueId_DeveArmazenarCorretamente()
    {
        // Arrange
        var insumoOS = new InsumoOS();
        var estoqueIdEsperado = Guid.NewGuid();

        // Act
        insumoOS.EstoqueId = estoqueIdEsperado;

        // Assert
        insumoOS.EstoqueId.Should().Be(estoqueIdEsperado, "o EstoqueId deve ser armazenado corretamente");
    }

    [Fact]
    public void InsumoOS_QuandoDefinidaQuantidade_DeveArmazenarCorretamente()
    {
        // Arrange
        var insumoOS = new InsumoOS();
        var quantidadeEsperada = 5;

        // Act
        insumoOS.Quantidade = quantidadeEsperada;

        // Assert
        insumoOS.Quantidade.Should().Be(quantidadeEsperada, "a quantidade deve ser armazenada corretamente");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void InsumoOS_QuandoDefinidaQuantidadePositiva_DeveArmazenarCorretamente(int quantidade)
    {
        // Arrange
        var insumoOS = new InsumoOS();

        // Act
        insumoOS.Quantidade = quantidade;

        // Assert
        insumoOS.Quantidade.Should().Be(quantidade, "deve aceitar qualquer quantidade positiva");
    }

    [Fact]
    public void InsumoOS_QuandoDefinidoOrdemServico_DeveArmazenarReferencia()
    {
        // Arrange
        var insumoOS = new InsumoOS();
        var ordemServico = new OrdemServico { Descricao = "Troca de óleo" };

        // Act
        insumoOS.OrdemServico = ordemServico;

        // Assert
        insumoOS.OrdemServico.Should().Be(ordemServico, "a referência da ordem de serviço deve ser armazenada corretamente");
        insumoOS.OrdemServico.Descricao.Should().Be("Troca de óleo", "as propriedades da ordem de serviço devem ser preservadas");
    }

    [Fact]
    public void InsumoOS_QuandoDefinidoEstoque_DeveArmazenarReferencia()
    {
        // Arrange
        var insumoOS = new InsumoOS();
        var estoque = new Estoque { Insumo = "Óleo Motor 5W30" };

        // Act
        insumoOS.Estoque = estoque;

        // Assert
        insumoOS.Estoque.Should().Be(estoque, "a referência do estoque deve ser armazenada corretamente");
        insumoOS.Estoque.Insumo.Should().Be("Óleo Motor 5W30", "as propriedades do estoque devem ser preservadas");
    }

    [Fact]
    public void InsumoOS_QuandoComparadoComOutroInsumoOSComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var insumoOS1 = new InsumoOS { Id = id, Quantidade = 5 };
        var insumoOS2 = new InsumoOS { Id = id, Quantidade = 10 };

        // Act & Assert
        insumoOS1.Should().Be(insumoOS2, "insumos OS com mesmo Id devem ser considerados iguais");
        insumoOS1.GetHashCode().Should().Be(insumoOS2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }

    [Fact]
    public void InsumoOS_QuandoComparadoComInsumoOSComIdDiferente_NaoDeveSerIgual()
    {
        // Arrange
        var insumoOS1 = new InsumoOS { Id = Guid.NewGuid(), Quantidade = 5 };
        var insumoOS2 = new InsumoOS { Id = Guid.NewGuid(), Quantidade = 5 };

        // Act & Assert
        insumoOS1.Should().NotBe(insumoOS2, "insumos OS com Ids diferentes não devem ser iguais");
    }

    [Fact]
    public void InsumoOS_QuandoDesativado_DeveMarcarComoInativo()
    {
        // Arrange
        var insumoOS = new InsumoOS { Ativo = true };

        // Act
        insumoOS.Desativar();

        // Assert
        insumoOS.Ativo.Should().BeFalse("o insumo OS deve estar marcado como inativo");
    }

    [Fact]
    public void InsumoOS_QuandoAtivado_DeveMarcarComoAtivo()
    {
        // Arrange
        var insumoOS = new InsumoOS { Ativo = false };

        // Act
        insumoOS.Ativar();

        // Assert
        insumoOS.Ativo.Should().BeTrue("o insumo OS deve estar marcado como ativo");
    }
}
