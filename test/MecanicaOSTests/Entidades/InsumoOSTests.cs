using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Entidades
{
    public class InsumoOSTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarInsumoOS_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var ordemServicoMock = new Mock<OrdemServico>();
            var estoqueId = Guid.NewGuid();
            var estoqueMock = new Mock<Estoque>();
            var quantidade = 5;

            // Act
            var insumoOS = new InsumoOS
            {
                OrdemServicoId = ordemServicoId,
                OrdemServico = ordemServicoMock.Object,
                EstoqueId = estoqueId,
                Estoque = estoqueMock.Object,
                Quantidade = quantidade
            };

            // Assert
            insumoOS.Should().NotBeNull();
            insumoOS.OrdemServicoId.Should().Be(ordemServicoId);
            insumoOS.OrdemServico.Should().BeEquivalentTo(ordemServicoMock.Object);
            insumoOS.EstoqueId.Should().Be(estoqueId);
            insumoOS.Estoque.Should().BeEquivalentTo(estoqueMock.Object);
            insumoOS.Quantidade.Should().Be(quantidade);
        }
    }
}
