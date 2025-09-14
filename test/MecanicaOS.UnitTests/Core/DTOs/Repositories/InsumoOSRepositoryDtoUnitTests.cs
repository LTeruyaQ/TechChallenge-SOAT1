using Xunit;
using FluentAssertions;
using Core.DTOs.Repositories.OrdemServicos;
using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Estoque;

namespace MecanicaOS.UnitTests.Core.DTOs.Repositories;

public class InsumoOSRepositoryDtoUnitTests
{
    [Fact]
    public void InsumoOSRepositoryDto_QuandoCriado_DeveHerdarDeRepositoryDto()
    {
        // Arrange & Act
        var dto = new InsumoOSRepositoryDto();

        // Assert
        dto.Should().BeAssignableTo<RepositoryDto>("InsumoOSRepositoryDto deve herdar de RepositoryDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void InsumoOSRepositoryDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
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
    public void InsumoOSRepositoryDto_QuandoDefinidoOrdemServicoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
        var ordemServicoIdEsperado = Guid.NewGuid();

        // Act
        dto.OrdemServicoId = ordemServicoIdEsperado;

        // Assert
        dto.OrdemServicoId.Should().Be(ordemServicoIdEsperado, "o OrdemServicoId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void InsumoOSRepositoryDto_QuandoDefinidoEstoqueId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
        var estoqueIdEsperado = Guid.NewGuid();

        // Act
        dto.EstoqueId = estoqueIdEsperado;

        // Assert
        dto.EstoqueId.Should().Be(estoqueIdEsperado, "o EstoqueId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void InsumoOSRepositoryDto_QuandoDefinidaQuantidade_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
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
    public void InsumoOSRepositoryDto_QuandoDefinidaQuantidadePositiva_DeveArmazenarCorretamente(int quantidade)
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();

        // Act
        dto.Quantidade = quantidade;

        // Assert
        dto.Quantidade.Should().Be(quantidade, "deve aceitar qualquer quantidade positiva no DTO");
    }

    [Fact]
    public void InsumoOSRepositoryDto_QuandoDefinidoOrdemServico_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
        var ordemServicoDto = new OrdemServicoRepositoryDto { Descricao = "Troca de óleo" };

        // Act
        dto.OrdemServico = ordemServicoDto;

        // Assert
        dto.OrdemServico.Should().Be(ordemServicoDto, "a referência da ordem de serviço deve ser armazenada corretamente no DTO");
        dto.OrdemServico.Descricao.Should().Be("Troca de óleo", "as propriedades da ordem de serviço devem ser preservadas na referência");
    }

    [Fact]
    public void InsumoOSRepositoryDto_QuandoDefinidoEstoque_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
        var estoqueDto = new EstoqueRepositoryDto { Insumo = "Óleo Motor 5W30" };

        // Act
        dto.Estoque = estoqueDto;

        // Assert
        dto.Estoque.Should().Be(estoqueDto, "a referência do estoque deve ser armazenada corretamente no DTO");
        dto.Estoque.Insumo.Should().Be("Óleo Motor 5W30", "as propriedades do estoque devem ser preservadas na referência");
    }

    [Fact]
    public void InsumoOSRepositoryDto_QuandoDefinidoOrdemServicoIdVazio_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
        var ordemServicoIdVazio = Guid.Empty;

        // Act
        dto.OrdemServicoId = ordemServicoIdVazio;

        // Assert
        dto.OrdemServicoId.Should().Be(ordemServicoIdVazio, "deve aceitar OrdemServicoId vazio no DTO");
    }

    [Fact]
    public void InsumoOSRepositoryDto_QuandoDefinidoEstoqueIdVazio_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
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
    public void InsumoOSRepositoryDto_QuandoDefinidoEstoqueComDiferentesInsumos_DevePreservarPropriedades(string insumo, int quantidade)
    {
        // Arrange
        var dto = new InsumoOSRepositoryDto();
        var estoqueDto = new EstoqueRepositoryDto 
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
