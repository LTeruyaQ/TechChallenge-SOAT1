namespace MecanicaOSTests.Dominio.Especificacoes.Estoque
{
    public class EstoqueEspecificacaoTests
    {
        private List<global::Dominio.Entidades.Estoque> GetEstoqueDeTeste()
        {
            return new List<global::Dominio.Entidades.Estoque>
            {
                new global::Dominio.Entidades.Estoque { Id = Guid.NewGuid(), Insumo = "Oleo", Preco = 50, QuantidadeDisponivel = 10, QuantidadeMinima = 5 },
                new global::Dominio.Entidades.Estoque { Id = Guid.NewGuid(), Insumo = "Filtro", Preco = 20, QuantidadeDisponivel = 3, QuantidadeMinima = 5 }, // Estoque crítico
                new global::Dominio.Entidades.Estoque { Id = Guid.NewGuid(), Insumo = "Vela", Preco = 15, QuantidadeDisponivel = 2, QuantidadeMinima = 10 } // Estoque crítico
            };
        }

        private List<global::Dominio.Entidades.AlertaEstoque> GetAlertasDeTeste(Guid estoqueId)
        {
            return new List<global::Dominio.Entidades.AlertaEstoque>
            {
                new global::Dominio.Entidades.AlertaEstoque { Id = Guid.NewGuid(), EstoqueId = estoqueId, DataCadastro = DateTime.UtcNow },
                new global::Dominio.Entidades.AlertaEstoque { Id = Guid.NewGuid(), EstoqueId = Guid.NewGuid(), DataCadastro = DateTime.UtcNow },
                new global::Dominio.Entidades.AlertaEstoque { Id = Guid.NewGuid(), EstoqueId = estoqueId, DataCadastro = DateTime.UtcNow.AddDays(-1) }
            };
        }

        [Fact]
        public void ObterAlertaDoDiaPorEstoqueEspecificacao_DeveRetornarAlertaCorreto()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var especificacao = new ObterAlertaDoDiaPorEstoqueEspecificacao(estoqueId, DateTime.UtcNow);
            var alertas = GetAlertasDeTeste(estoqueId);

            // Act
            var resultado = alertas.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(estoqueId, resultado.First().EstoqueId);
        }

        [Fact]
        public void ObterEstoqueCriticoEspecificacao_DeveRetornarEstoquesCriticos()
        {
            // Arrange
            var especificacao = new ObterEstoqueCriticoEspecificacao();
            var estoques = GetEstoqueDeTeste();

            // Act
            var resultado = estoques.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, e => Assert.True(e.QuantidadeDisponivel <= e.QuantidadeMinima));
        }
    }
}
