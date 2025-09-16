using Core.DTOs.UseCases.Estoque;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.DTOs.UseCases;

public class EstoqueUseCaseDtoUnitTests
{
    [Fact]
    public void CadastrarEstoqueUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarCadastrarEstoqueUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Insumo.Should().Be("Óleo Motor 5W30", "o insumo deve ser armazenado corretamente");
        dto.Descricao.Should().Be("Óleo sintético para motor 5W30 - 1 litro",
            "a descrição deve ser armazenada corretamente");
        dto.Preco.Should().Be(45.90m, "o preço deve ser armazenado corretamente");
        dto.QuantidadeDisponivel.Should().Be(50, "a quantidade disponível deve ser armazenada corretamente");
        dto.QuantidadeMinima.Should().Be(10, "a quantidade mínima deve ser armazenada corretamente");
    }

    [Fact]
    public void CadastrarEstoqueUseCaseDto_DevePermitirDescricaoNula()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarCadastrarEstoqueUseCaseDtoSemDescricao();

        // Assert
        dto.Descricao.Should().BeNull("deve permitir descrição nula");
        dto.Insumo.Should().Be("Filtro de Ar", "deve manter insumo obrigatório");
        dto.Preco.Should().Be(25.00m, "deve manter preço válido");
        dto.QuantidadeDisponivel.Should().BeGreaterThan(0, "deve ter quantidade disponível");
        dto.QuantidadeMinima.Should().BeGreaterOrEqualTo(0, "deve ter quantidade mínima válida");
    }

    [Fact]
    public void CadastrarEstoqueUseCaseDto_DevePermitirEstoqueBaixo()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarCadastrarEstoqueUseCaseDtoEstoqueBaixo();

        // Assert
        dto.QuantidadeDisponivel.Should().Be(3, "deve aceitar quantidade baixa");
        dto.QuantidadeMinima.Should().Be(5, "quantidade mínima pode ser maior que disponível");
        dto.QuantidadeDisponivel.Should().BeLessThan(dto.QuantidadeMinima,
            "deve permitir estoque abaixo do mínimo");
        dto.Insumo.Should().Be("Pastilha de Freio", "deve manter insumo");
        dto.Preco.Should().Be(120.00m, "deve manter preço");
    }

    [Fact]
    public void CadastrarEstoqueUseCaseDto_DevePermitirProdutoGratuito()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarCadastrarEstoqueUseCaseDtoGratuito();

        // Assert
        dto.Preco.Should().Be(0.00m, "deve aceitar preço zero");
        dto.QuantidadeMinima.Should().Be(0, "deve aceitar quantidade mínima zero");
        dto.Insumo.Should().Be("Amostra Grátis", "deve aceitar produtos gratuitos");
        dto.QuantidadeDisponivel.Should().BeGreaterThan(0, "deve ter quantidade disponível");
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(10.50)]
    [InlineData(999.99)]
    [InlineData(1500.00)]
    public void CadastrarEstoqueUseCaseDto_DeveAceitarDiferentesPrecos(decimal preco)
    {
        // Arrange & Act
        var dto = new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Produto Teste",
            Descricao = "Descrição Teste",
            Preco = preco,
            QuantidadeDisponivel = 10,
            QuantidadeMinima = 5
        };

        // Assert
        dto.Preco.Should().Be(preco, "deve aceitar o preço especificado");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(50, 10)]
    [InlineData(100, 20)]
    public void CadastrarEstoqueUseCaseDto_DeveAceitarDiferentesQuantidades(int disponivel, int minima)
    {
        // Arrange & Act
        var dto = new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Produto Teste",
            Descricao = "Descrição Teste",
            Preco = 50.00m,
            QuantidadeDisponivel = disponivel,
            QuantidadeMinima = minima
        };

        // Assert
        dto.QuantidadeDisponivel.Should().Be(disponivel, "deve aceitar a quantidade disponível especificada");
        dto.QuantidadeMinima.Should().Be(minima, "deve aceitar a quantidade mínima especificada");
    }

    [Fact]
    public void AtualizarEstoqueUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarAtualizarEstoqueUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Insumo.Should().Be("Óleo Motor 10W40 Atualizado", "o insumo deve ser armazenado corretamente");
        dto.Descricao.Should().Be("Óleo mineral para motor 10W40 - 1 litro",
            "a descrição deve ser armazenada corretamente");
        dto.Preco.Should().Be(35.50m, "o preço deve ser armazenado corretamente");
        dto.QuantidadeDisponivel.Should().Be(75, "a quantidade disponível deve ser armazenada corretamente");
        dto.QuantidadeMinima.Should().Be(15, "a quantidade mínima deve ser armazenada corretamente");
    }

    [Fact]
    public void AtualizarEstoqueUseCaseDto_DevePermitirCamposNulos()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarAtualizarEstoqueUseCaseDtoComCamposNulos();

        // Assert
        dto.Insumo.Should().BeNull("deve permitir insumo nulo");
        dto.Descricao.Should().BeNull("deve permitir descrição nula");
        dto.Preco.Should().BeNull("deve permitir preço nulo");
        dto.QuantidadeDisponivel.Should().BeNull("deve permitir quantidade disponível nula");
        dto.QuantidadeMinima.Should().BeNull("deve permitir quantidade mínima nula");
    }

    [Fact]
    public void AtualizarEstoqueUseCaseDto_DevePermitirAtualizacaoParcialPreco()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarAtualizarEstoqueUseCaseDtoApenasPreco();

        // Assert
        dto.Preco.Should().Be(55.00m, "deve permitir atualização apenas do preço");
        dto.Insumo.Should().BeNull("outros campos podem permanecer nulos");
        dto.QuantidadeDisponivel.Should().BeNull("outros campos podem permanecer nulos");
    }

    [Fact]
    public void AtualizarEstoqueUseCaseDto_DevePermitirAtualizacaoParcialQuantidade()
    {
        // Arrange & Act
        var dto = EstoqueUseCaseFixture.CriarAtualizarEstoqueUseCaseDtoApenasQuantidade();

        // Assert
        dto.QuantidadeDisponivel.Should().Be(200, "deve permitir atualização apenas da quantidade");
        dto.Preco.Should().BeNull("outros campos podem permanecer nulos");
        dto.Insumo.Should().BeNull("outros campos podem permanecer nulos");
    }

    [Fact]
    public void CadastrarEstoqueUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = EstoqueUseCaseFixture.CriarListaCadastrarEstoqueUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(4, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Insumo),
            "todos os DTOs devem ter insumo");
        lista.Should().OnlyContain(dto => dto.Preco >= 0,
            "todos os preços devem ser não negativos");
        lista.Should().OnlyContain(dto => dto.QuantidadeDisponivel >= 0,
            "todas as quantidades disponíveis devem ser não negativas");
        lista.Should().OnlyContain(dto => dto.QuantidadeMinima >= 0,
            "todas as quantidades mínimas devem ser não negativas");
    }

    [Fact]
    public void AtualizarEstoqueUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = EstoqueUseCaseFixture.CriarListaAtualizarEstoqueUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(4, "deve conter todos os DTOs da fixture");
    }

    [Fact]
    public void EstoqueUseCaseDto_DevePermitirAlteracaoDePropriedades()
    {
        // Arrange
        var dto = EstoqueUseCaseFixture.CriarCadastrarEstoqueUseCaseDtoValido();
        var novoInsumo = "Novo Insumo";
        var novoPreco = 99.99m;

        // Act
        dto.Insumo = novoInsumo;
        dto.Preco = novoPreco;

        // Assert
        dto.Insumo.Should().Be(novoInsumo, "deve permitir alteração do insumo");
        dto.Preco.Should().Be(novoPreco, "deve permitir alteração do preço");
    }

    [Fact]
    public void EstoqueUseCaseDto_DeveSerDistintoEntreInstancias()
    {
        // Arrange & Act
        var dto1 = EstoqueUseCaseFixture.CriarCadastrarEstoqueUseCaseDtoValido();
        var dto2 = EstoqueUseCaseFixture.CriarCadastrarEstoqueUseCaseDtoEstoqueBaixo();

        // Assert
        dto1.Should().NotBeSameAs(dto2, "devem ser instâncias diferentes");
        dto1.Insumo.Should().NotBe(dto2.Insumo, "devem ter insumos diferentes");
        dto1.Preco.Should().NotBe(dto2.Preco, "devem ter preços diferentes");
        dto1.QuantidadeDisponivel.Should().NotBe(dto2.QuantidadeDisponivel,
            "devem ter quantidades disponíveis diferentes");
    }
}
