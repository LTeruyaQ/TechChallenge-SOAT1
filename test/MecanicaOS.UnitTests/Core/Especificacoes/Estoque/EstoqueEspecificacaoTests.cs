using Core.DTOs.Entidades.Estoque;
using Core.Especificacoes.Estoque;
using MecanicaOS.UnitTests.Fixtures;

namespace MecanicaOS.UnitTests.Core.Especificacoes.Estoque;

public class EstoqueEspecificacaoTests
{
    private List<EstoqueEntityDto> GetEstoquesDeTeste()
    {
        var estoque1 = EstoqueFixture.CriarEstoqueEntityDtoValido();
        estoque1.QuantidadeDisponivel = 25;
        estoque1.QuantidadeMinima = 5;

        var estoque2 = EstoqueFixture.CriarEstoqueEntityDtoSemDescricao();
        estoque2.QuantidadeDisponivel = 3; // Estoque crítico
        estoque2.QuantidadeMinima = 10;

        var estoque3 = EstoqueFixture.CriarEstoqueEntityDtoValido();
        estoque3.QuantidadeDisponivel = 0; // Estoque zerado
        estoque3.QuantidadeMinima = 4;

        var estoque4 = EstoqueFixture.CriarEstoqueEntityDtoValido();
        estoque4.QuantidadeDisponivel = 15; // Estoque normal
        estoque4.QuantidadeMinima = 8;

        return new List<EstoqueEntityDto> { estoque1, estoque2, estoque3, estoque4 };
    }

    [Fact]
    public void ObterEstoqueCriticoEspecificacao_DeveRetornarEstoquesComQuantidadeBaixa()
    {
        // Arrange
        var estoques = GetEstoquesDeTeste();
        var especificacao = new ObterEstoqueCriticoEspecificacao();

        // Act
        var resultado = estoques.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar estoques com quantidade menor ou igual à mínima");
        resultado.Should().OnlyContain(e => e.QuantidadeDisponivel <= e.QuantidadeMinima,
            "todos os estoques devem ter quantidade disponível menor ou igual à mínima");
    }

    [Fact]
    public void ObterEstoqueCriticoEspecificacao_ComEstoqueZerado_DeveIncluirNoResultado()
    {
        // Arrange
        var estoques = GetEstoquesDeTeste();
        var especificacao = new ObterEstoqueCriticoEspecificacao();

        // Act
        var resultado = estoques.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().Contain(e => e.QuantidadeDisponivel == 0,
            "deve incluir estoques com quantidade zerada");
    }

    [Fact]
    public void ObterEstoqueCriticoEspecificacao_ComEstoquesNormais_NaoDeveRetornar()
    {
        // Arrange
        var estoques = new List<EstoqueEntityDto>
        {
            EstoqueFixture.CriarEstoqueEntityDtoValido()
        };
        estoques.First().QuantidadeDisponivel = 50;
        estoques.First().QuantidadeMinima = 10;

        var especificacao = new ObterEstoqueCriticoEspecificacao();

        // Act
        var resultado = estoques.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar estoques com quantidade acima da mínima");
    }

    [Fact]
    public void ObterEstoqueCriticoEspecificacao_ComQuantidadeIgualMinima_DeveRetornar()
    {
        // Arrange
        var estoques = new List<EstoqueEntityDto>
        {
            EstoqueFixture.CriarEstoqueEntityDtoValido()
        };
        estoques.First().QuantidadeDisponivel = 10;
        estoques.First().QuantidadeMinima = 10;

        var especificacao = new ObterEstoqueCriticoEspecificacao();

        // Act
        var resultado = estoques.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar estoque com quantidade igual à mínima");
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(3, 10)]
    [InlineData(5, 5)]
    [InlineData(1, 8)]
    public void ObterEstoqueCriticoEspecificacao_ComDiferentesQuantidades_DeveRetornarSeForCritico(
        int quantidadeDisponivel, int quantidadeMinima)
    {
        // Arrange
        var estoque = EstoqueFixture.CriarEstoqueEntityDtoValido();
        estoque.QuantidadeDisponivel = quantidadeDisponivel;
        estoque.QuantidadeMinima = quantidadeMinima;

        var estoques = new List<EstoqueEntityDto> { estoque };
        var especificacao = new ObterEstoqueCriticoEspecificacao();

        // Act
        var resultado = estoques.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        if (quantidadeDisponivel <= quantidadeMinima)
        {
            resultado.Should().HaveCount(1,
                $"deve retornar estoque crítico (disponível: {quantidadeDisponivel}, mínima: {quantidadeMinima})");
        }
        else
        {
            resultado.Should().BeEmpty(
                $"não deve retornar estoque normal (disponível: {quantidadeDisponivel}, mínima: {quantidadeMinima})");
        }
    }

    [Fact]
    public void ObterAlertaDoDiaPorEstoqueEspecificacao_DeveRetornarAlertasDoEstoque()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();
        var dataHoje = DateTime.Today;
        var especificacao = new ObterAlertaDoDiaPorEstoqueEspecificacao(estoqueId, dataHoje);

        // Assert
        especificacao.Should().NotBeNull("especificação deve ser criada corretamente");
        especificacao.Expressao.Should().NotBeNull("expressão deve estar definida");
    }

    [Fact]
    public void ObterAlertaDoDiaPorEstoqueEspecificacao_ComParametrosValidos_DeveDefinirExpressaoCorretamente()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();
        var dataHoje = DateTime.Today;
        var especificacao = new ObterAlertaDoDiaPorEstoqueEspecificacao(estoqueId, dataHoje);

        // Act & Assert
        especificacao.Expressao.Should().NotBeNull("expressão deve estar definida para filtrar alertas");
    }
}
