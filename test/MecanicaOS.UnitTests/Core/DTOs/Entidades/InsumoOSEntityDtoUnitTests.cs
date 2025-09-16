using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;

namespace MecanicaOS.UnitTests.Core.DTOs.Entidades;

public class InsumoOSEntityDtoUnitTests
{
    [Fact]
    public void InsumoOSEntityDto_QuandoCriado_DeveHerdarDeEntityDto()
    {
        // Arrange & Act
        var dto = new InsumoOSEntityDto();

        // Assert
        dto.Should().BeAssignableTo<EntityDto>("InsumoOSEntityDto deve herdar de EntityDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void InsumoOSEntityDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
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
    public void InsumoOSEntityDto_QuandoDefinidoOrdemServicoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var ordemServicoIdEsperado = Guid.NewGuid();

        // Act
        dto.OrdemServicoId = ordemServicoIdEsperado;

        // Assert
        dto.OrdemServicoId.Should().Be(ordemServicoIdEsperado, "o OrdemServicoId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void InsumoOSEntityDto_QuandoDefinidoEstoqueId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var estoqueIdEsperado = Guid.NewGuid();

        // Act
        dto.EstoqueId = estoqueIdEsperado;

        // Assert
        dto.EstoqueId.Should().Be(estoqueIdEsperado, "o EstoqueId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void InsumoOSEntityDto_QuandoDefinidaQuantidade_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var quantidadeEsperada = 5;

        // Act
        dto.Quantidade = quantidadeEsperada;

        // Assert
        dto.Quantidade.Should().Be(quantidadeEsperada, "a quantidade deve ser armazenada corretamente no DTO");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void InsumoOSEntityDto_QuandoDefinidaQuantidadePositiva_DeveArmazenarCorretamente(int quantidade)
    {
        // Arrange
        var dto = new InsumoOSEntityDto();

        // Act
        dto.Quantidade = quantidade;

        // Assert
        dto.Quantidade.Should().Be(quantidade, "deve aceitar qualquer quantidade positiva no DTO");
    }

    [Fact]
    public void InsumoOSEntityDto_QuandoDefinidoOrdemServico_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var ordemServicoDto = new OrdemServicoEntityDto { Descricao = "Troca de óleo" };

        // Act
        dto.OrdemServico = ordemServicoDto;

        // Assert
        dto.OrdemServico.Should().Be(ordemServicoDto, "a referência da ordem de serviço deve ser armazenada corretamente no DTO");
        dto.OrdemServico.Descricao.Should().Be("Troca de óleo", "as propriedades da ordem de serviço devem ser preservadas na referência");
    }

    [Fact]
    public void InsumoOSEntityDto_QuandoDefinidoEstoque_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var estoqueDto = new EstoqueEntityDto { Insumo = "Óleo Motor 5W30" };

        // Act
        dto.Estoque = estoqueDto;

        // Assert
        dto.Estoque.Should().Be(estoqueDto, "a referência do estoque deve ser armazenada corretamente no DTO");
        dto.Estoque.Insumo.Should().Be("Óleo Motor 5W30", "as propriedades do estoque devem ser preservadas na referência");
    }

    [Fact]
    public void InsumoOSEntityDto_QuandoDefinidoOrdemServicoIdVazio_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var ordemServicoIdVazio = Guid.Empty;

        // Act
        dto.OrdemServicoId = ordemServicoIdVazio;

        // Assert
        dto.OrdemServicoId.Should().Be(ordemServicoIdVazio, "deve aceitar OrdemServicoId vazio no DTO");
    }

    [Fact]
    public void InsumoOSEntityDto_QuandoDefinidoEstoqueIdVazio_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var estoqueIdVazio = Guid.Empty;

        // Act
        dto.EstoqueId = estoqueIdVazio;

        // Assert
        dto.EstoqueId.Should().Be(estoqueIdVazio, "deve aceitar EstoqueId vazio no DTO");
    }

    [Theory]
    [InlineData("Óleo Motor 5W30", 2)]
    [InlineData("Filtro de Ar", 1)]
    [InlineData("Pastilha de Freio", 4)]
    public void InsumoOSEntityDto_QuandoDefinidoEstoqueComDiferentesInsumos_DevePreservarPropriedades(string insumo, int quantidade)
    {
        // Arrange
        var dto = new InsumoOSEntityDto();
        var estoqueDto = new EstoqueEntityDto
        {
            Insumo = insumo,
            Preco = 50.00m,
            QuantidadeDisponivel = 10
        };

        // Act
        dto.Estoque = estoqueDto;
        dto.Quantidade = quantidade;

        // Assert
        dto.Estoque.Insumo.Should().Be(insumo, "o insumo deve ser preservado na referência");
        dto.Estoque.Preco.Should().Be(50.00m, "o preço deve ser preservado na referência");
        dto.Estoque.QuantidadeDisponivel.Should().Be(10, "a quantidade disponível deve ser preservada na referência");
        dto.Quantidade.Should().Be(quantidade, "a quantidade do insumo OS deve ser armazenada corretamente");
    }
}
