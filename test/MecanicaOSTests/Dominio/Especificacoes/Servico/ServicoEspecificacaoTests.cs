namespace MecanicaOSTests.Dominio.Especificacoes.Servico
{
    public class ServicoEspecificacaoTests
    {
        private List<global::Dominio.Entidades.Servico> GetServicosDeTeste()
        {
            return new List<global::Dominio.Entidades.Servico>
            {
                new global::Dominio.Entidades.Servico { Id = Guid.NewGuid(), Nome = "Troca de Oleo", Descricao = "Troca de Oleo do Motor", Valor = 100, Disponivel = true },
                new global::Dominio.Entidades.Servico { Id = Guid.NewGuid(), Nome = "Alinhamento", Descricao = "Alinhamento e Balanceamento", Valor = 150, Disponivel = true },
                new global::Dominio.Entidades.Servico { Id = Guid.NewGuid(), Nome = "Troca de Pneu", Descricao = "Troca de um pneu", Valor = 50, Disponivel = false }
            };
        }

        [Fact]
        public void ObterServicoDisponivelEspecificacao_DeveRetornarServicosDisponiveis()
        {
            // Arrange
            var especificacao = new ObterServicoDisponivelEspecificacao();
            var servicos = GetServicosDeTeste();

            // Act
            var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, s => Assert.True(s.Disponivel));
        }

        [Fact]
        public void ObterServicoPorNomeEspecificacao_DeveRetornarServicoCorreto()
        {
            // Arrange
            var nome = "Troca de Oleo";
            var especificacao = new ObterServicoPorNomeEspecificacao(nome);
            var servicos = GetServicosDeTeste();

            // Act
            var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(nome, resultado.First().Nome);
        }
    }
}
