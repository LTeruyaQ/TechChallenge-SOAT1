namespace MecanicaOSTests.Dominio.Especificacoes.Base.Extensoes
{
    public class OuExtensaoTests
    {
        [Fact]
        public void Ou_DeveRetornarOuEspecificacaoComAsEspecificacoesCorretas()
        {
            // Arrange
            var spec1 = new TestSpecification(x => x.Id > 0);
            var spec2 = new TestSpecification(x => x.Id < 10);

            // Act
            var ouSpec = spec1.Ou(spec2);

            // Assert
            Assert.NotNull(ouSpec);
            var ouEspecificacao = Assert.IsType<OuEspecificacao<TestEntity>>(ouSpec);
            Assert.Equal(spec1, ouEspecificacao.Esquerda);
            Assert.Equal(spec2, ouEspecificacao.Direita);
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
    }

    public class TestSpecification : EspecificacaoBase<TestEntity>
    {
        public TestSpecification(System.Linq.Expressions.Expression<System.Func<TestEntity, bool>> expression)
        {
            Expressao = expression;
        }

        public override System.Linq.Expressions.Expression<System.Func<TestEntity, bool>> Expressao { get; }
    }
}
