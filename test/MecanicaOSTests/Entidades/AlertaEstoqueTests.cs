using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Entidades
{
    public class AlertaEstoqueTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarAlertaEstoque_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueMock = new Mock<Estoque>();

            // Act
            var alertaEstoque = new AlertaEstoque
            {
                EstoqueId = estoqueId,
                Estoque = estoqueMock.Object
            };

            // Assert
            alertaEstoque.Should().NotBeNull();
            alertaEstoque.EstoqueId.Should().Be(estoqueId);
            alertaEstoque.Estoque.Should().BeEquivalentTo(estoqueMock.Object);
        }
    }
}
