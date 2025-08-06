using Dominio.Entidades;
using FluentAssertions;
using MecanicaOSTests.Fixtures;

namespace MecanicaOSTests.Entidades
{
    public class AlertaEstoqueTests
    {
        private readonly EstoqueFixture _estoqueFixture;

        public AlertaEstoqueTests()
        {
            _estoqueFixture = new EstoqueFixture();
        }

        [Fact]
        public void Dado_DadosValidos_Quando_CriarAlertaEstoque_Entao_DeveCriarComSucesso()
        {
            //Arrange
            var estoque = _estoqueFixture.CriarEstoqueValido();

            //Act
            var alertaEstoque = new AlertaEstoque
            {
                EstoqueId = estoque.Id,
                Estoque = estoque
            };

            //Assert
            alertaEstoque.EstoqueId.Should().Be(estoque.Id);
            alertaEstoque.Estoque.Should().Be(estoque);
        }
    }
}
