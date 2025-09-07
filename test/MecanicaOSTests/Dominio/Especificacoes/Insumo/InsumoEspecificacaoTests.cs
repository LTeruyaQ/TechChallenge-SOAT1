using Dominio.Entidades;
using Dominio.Especificacoes.Insumo;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MecanicaOSTests.Dominio.Especificacoes.Insumo
{
    public class InsumoEspecificacaoTests
    {
        private List<global::Dominio.Entidades.InsumoOS> GetInsumosDeTeste()
        {
            var osId = Guid.NewGuid();
            var estoque1 = new global::Dominio.Entidades.Estoque("Oleo", string.Empty, 50, 10, 5);
            var estoque2 = new global::Dominio.Entidades.Estoque("Filtro", string.Empty, 20, 100, 5); // Estoque crítico
            var estoque3 = new global::Dominio.Entidades.Estoque("Vela", string.Empty, 15, 200, 10);

            return new List<global::Dominio.Entidades.InsumoOS>
            {
                new global::Dominio.Entidades.InsumoOS { Id = Guid.NewGuid(), OrdemServicoId = osId, EstoqueId = estoque1.Id, Estoque = estoque1 },
                new global::Dominio.Entidades.InsumoOS { Id = Guid.NewGuid(), OrdemServicoId = osId, EstoqueId = estoque2.Id, Estoque = estoque2 },
                new global::Dominio.Entidades.InsumoOS { Id = Guid.NewGuid(), OrdemServicoId = Guid.NewGuid(), EstoqueId = estoque3.Id, Estoque = estoque3 }
            };
        }

        [Fact]
        public void ObterInsumosPorIdsEOSEspecificacao_DeveRetornarInsumosCorretos()
        {
            // Arrange
            var insumos = GetInsumosDeTeste();
            var osId = insumos.First().OrdemServicoId;
            var idsInsumos = insumos.Where(i => i.OrdemServicoId == osId).Select(i => i.Id).ToList();
            var especificacao = new ObterInsumosPorIdsEOSEspecificacao(osId, idsInsumos);

            // Act
            var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, i => Assert.Equal(osId, i.OrdemServicoId));
            Assert.All(resultado, i => Assert.Contains(i.Id, idsInsumos));
        }
    }
}
