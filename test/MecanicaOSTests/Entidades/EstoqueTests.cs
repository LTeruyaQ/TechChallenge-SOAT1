using Dominio.Entidades;
using FluentAssertions;

namespace MecanicaOSTests.Entidades
{
    public class EstoqueTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarEstoque_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var insumo = "Peça Teste";
            var descricao = "Descrição da Peça";
            var preco = 100.50m;
            var quantidadeDisponivel = 10;
            var quantidadeMinima = 5;

            // Act
            var estoque = new Estoque
            (
                insumo,
                descricao,
                preco,
                quantidadeDisponivel,
                quantidadeMinima
            );

            // Assert
            estoque.Should().NotBeNull();
            estoque.Insumo.Should().Be(insumo);
            estoque.Descricao.Should().Be(descricao);
            estoque.Preco.Should().Be(preco);
            estoque.QuantidadeDisponivel.Should().Be(quantidadeDisponivel);
            estoque.QuantidadeMinima.Should().Be(quantidadeMinima);
        }
    }
}
