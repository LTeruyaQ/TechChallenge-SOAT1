using Aplicacao.Servicos;
using Dominio.Entidades;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

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
                    new InsumoOS { Quantidade = 2, Estoque = new Estoque("Óleo Motor", "Óleo sintético 5W30", (decimal)25, 10, 10) },
                    new InsumoOS { Quantidade = 1, Estoque = new Estoque("Óleo Motor", "Óleo sintético 5W30", (decimal)50, 10, 10) }
                }
            };

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            orcamento.Should().Be(200.00m); // 100 + (2*25) + (1*50)
        }
    }
}
