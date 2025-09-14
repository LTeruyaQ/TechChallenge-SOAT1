namespace MecanicaOSTests.Dominio.Especificacoes.Base
{
    public class OuEspecificacaoTests
    {
        private class TestSpecification : EspecificacaoBase<global::Dominio.Entidades.Cliente>
        {
            private readonly System.Linq.Expressions.Expression<System.Func<global::Dominio.Entidades.Cliente, bool>> _expression;

            public TestSpecification(System.Linq.Expressions.Expression<System.Func<global::Dominio.Entidades.Cliente, bool>> expression)
            {
                _expression = expression;
            }

            public override System.Linq.Expressions.Expression<System.Func<global::Dominio.Entidades.Cliente, bool>> Expressao => _expression;

            public new void AdicionarInclusao(System.Linq.Expressions.Expression<System.Func<global::Dominio.Entidades.Cliente, object>> include)
            {
                base.AdicionarInclusao(include);
            }
        }

        [Fact]
        public void Ou_DeveRetornarOuEspecificacaoComAsEspecificacoesCorretas()
        {
            // Arrange
            var spec1 = new TestSpecification(c => c.Nome == "Teste");
            var spec2 = new TestSpecification(c => c.Id == System.Guid.Empty);

            // Act
            var ouSpec = spec1.Ou(spec2);

            // Assert
            Assert.NotNull(ouSpec);
            var concreteSpec = Assert.IsType<OuEspecificacao<global::Dominio.Entidades.Cliente>>(ouSpec);
            Assert.Equal(spec1, concreteSpec.Esquerda);
            Assert.Equal(spec2, concreteSpec.Direita);
        }

        [Fact]
        public void Dado_DuasEspecificacoes_Quando_CombinadasComOuEspecificacao_Entao_DeveRetornarRegistrosQueAtendemPeloMenosUma()
        {
            // Arrange
            var clientes = new[]
            {
                new global::Dominio.Entidades.Cliente { Nome = "Teste" },
                new global::Dominio.Entidades.Cliente { Nome = "Outro" },
                new global::Dominio.Entidades.Cliente { Nome = "Teste 2" }
            }.AsQueryable();

            var spec1 = new TestSpecification(c => c.Nome == "Teste");
            var spec2 = new TestSpecification(c => c.Nome == "Outro");

            var ouSpec = spec1.Ou(spec2);

            // Act
            var resultado = clientes.Where(ouSpec.Expressao).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, c => c.Nome == "Teste");
            Assert.Contains(resultado, c => c.Nome == "Outro");
        }

        [Fact]
        public void Dado_EspecificacaoComInclusoes_Quando_CombinadaComOuEspecificacao_Entao_DeveManterTodasAsInclusoes()
        {
            // Arrange
            var spec1 = new TestSpecification(c => c.Nome == "Teste");
            spec1.AdicionarInclusao(c => c.Veiculos);

            var spec2 = new TestSpecification(c => c.Id == System.Guid.Empty);
            spec2.AdicionarInclusao(c => c.Contato);

            // Act
            var ouSpec = spec1.Ou(spec2);

            // Assert
            Assert.Contains("Veiculos", ouSpec.Inclusoes);
            Assert.Contains("Contato", ouSpec.Inclusoes);
        }

        [Fact]
        public void Dado_UmaEspecificacaoNula_Quando_CombinadaComOuEspecificacao_Entao_DeveLancarArgumentNullException()
        {
            // Arrange
            var spec1 = new TestSpecification(c => c.Nome == "Teste");
            TestSpecification spec2 = null;

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => spec1.Ou(spec2));
            Assert.Throws<System.ArgumentNullException>(() => spec2.Ou(spec1));
        }
    }
}
