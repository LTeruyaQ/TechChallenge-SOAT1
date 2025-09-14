using Xunit;
using FluentAssertions;
using Core.DTOs.Repositories.Estoque;
using Core.DTOs.Repositories.Autenticacao;

namespace MecanicaOS.UnitTests.Core.DTOs.Repositories;

public class AlertaEstoqueRepositoryDtoUnitTests
{
    [Fact]
    public void AlertaEstoqueRepositoryDto_QuandoCriado_DeveHerdarDeRepositoryDto()
    {
        // Arrange & Act
        var dto = new AlertaEstoqueRepositoryDto();

        // Assert
        dto.Should().BeAssignableTo<RepositoryDto>("AlertaEstoqueRepositoryDto deve herdar de RepositoryDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void AlertaEstoqueRepositoryDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new AlertaEstoqueRepositoryDto();
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
    public void AlertaEstoqueRepositoryDto_QuandoDefinidoEstoqueId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new AlertaEstoqueRepositoryDto();
        var estoqueIdEsperado = Guid.NewGuid();

        // Act
        dto.EstoqueId = estoqueIdEsperado;

        // Assert
        dto.EstoqueId.Should().Be(estoqueIdEsperado, "o EstoqueId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void AlertaEstoqueRepositoryDto_QuandoDefinidoEstoque_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new AlertaEstoqueRepositoryDto();
        var estoqueDto = new EstoqueRepositoryDto { Insumo = "Óleo Motor" };

        // Act
        dto.Estoque = estoqueDto;

        // Assert
        dto.Estoque.Should().Be(estoqueDto, "a referência do estoque deve ser armazenada corretamente no DTO");
        dto.Estoque.Insumo.Should().Be("Óleo Motor", "as propriedades do estoque devem ser preservadas na referência");
    }

    [Fact]
    public void AlertaEstoqueRepositoryDto_QuandoDefinidoEstoqueIdVazio_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new AlertaEstoqueRepositoryDto();
        var estoqueIdVazio = Guid.Empty;

        // Act
        dto.EstoqueId = estoqueIdVazio;

        // Assert
        dto.EstoqueId.Should().Be(estoqueIdVazio, "deve aceitar EstoqueId vazio no DTO");
    }

    [Theory]
    [InlineData("Óleo Motor 5W30")]
    [InlineData("Filtro de Ar")]
    [InlineData("Pastilha de Freio")]
    public void AlertaEstoqueRepositoryDto_QuandoDefinidoEstoqueComDiferentesInsumos_DevePreservarPropriedades(string insumo)
    {
        // Arrange
        var dto = new AlertaEstoqueRepositoryDto();
        var estoqueDto = new EstoqueRepositoryDto 
        { 
            Insumo = insumo,
            Preco = 50.00m,
            QuantidadeDisponivel = 5
        };

        // Act
        dto.Estoque = estoqueDto;

        // Assert
        dto.Estoque.Insumo.Should().Be(insumo, "o insumo deve ser preservado na referência");
        dto.Estoque.Preco.Should().Be(50.00m, "o preço deve ser preservado na referência");
        dto.Estoque.QuantidadeDisponivel.Should().Be(5, "a quantidade deve ser preservada na referência");
    }
}
