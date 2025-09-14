using FluentAssertions;

namespace MecanicaOSTests.Servicos
{
    public class OrcamentoServicoTests
    {
        private readonly OrcamentoServico _orcamentoServico;

        public OrcamentoServicoTests()
        {
            _orcamentoServico = new OrcamentoServico();
        }

        [Fact]
        public void GerarOrcamento_DeveCalcularCorretamente()
        {
            // Arrange
            var ordemServico = new OrdemServico
            {
                Servico = new Servico { Nome = "Teste", Descricao = "Teste", Valor = 100.00m },
                InsumosOS = new List<InsumoOS>
                {
                    new InsumoOS { Quantidade = 2, Estoque = new Estoque { Preco = 25.00m } },
                    new InsumoOS { Quantidade = 1, Estoque = new Estoque { Preco = 50.00m } }
                }
            };

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            orcamento.Should().Be(200.00m); // 100 + (2*25) + (1*50)
        }
    }
}
