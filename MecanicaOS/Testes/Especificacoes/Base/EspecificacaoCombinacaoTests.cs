using Dominio.Especificacoes.Base.Extensoes;
using Dominio.Especificacoes.Base.Interfaces;

namespace Testes.Especificacoes.Base
{
    public class EspecificacaoCombinacaoTests : EspecificacaoTestBase
    {
        [Fact]
        public void Deve_CombinarEspecificacoesComE_Quando_AmbasVerdadeiras()
        {
            // Arrange
            var espec1 = new ProdutoAtivoEspecificacao();
            var espec2 = new ProdutoCategoriaEspecificacao(1); // Eletrônicos
            var produtos = CriarProdutosParaTeste();

            // Act
            var resultado = produtos.Where(espec1.E(espec2).Expressao.Compile()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count); // Notebook e Smartphone
            Assert.All(resultado, p => Assert.True(p.Ativo && p.CategoriaId == 1));
        }

        [Fact]
        public void Deve_CombinarEspecificacoesComOU_Quando_QualquerUmaVerdadeira()
        {
            // Arrange
            var espec1 = new ProdutoAtivoEspecificacao();
            var espec2 = new ProdutoPrecoMinimoEspecificacao(2000);
            var produtos = CriarProdutosParaTeste();

            // Act
            var resultado = produtos.Where(espec1.Ou(espec2).Expressao.Compile()).ToList();

            // Assert
            Assert.Equal(4, resultado.Count); // Todos os ativos (1,2,3,5) + Notebook (preço >= 2000)
            Assert.Contains(resultado, p => p.Id == 1); // Notebook - ativo e preço >= 2000
            Assert.Contains(resultado, p => p.Id == 2); // Smartphone - ativo
            Assert.Contains(resultado, p => p.Id == 3); // Livro C# - ativo
            Assert.Contains(resultado, p => p.Id == 5); // Livro EF Core - ativo
        }

        [Fact]
        public void Deve_ManterInclusoes_Quando_CombinarEspecificacoes()
        {
            // Arrange
            var espec1 = new ProdutoAtivoEspecificacao()
                .Incluir(p => p.Categoria)
                .Incluir("Fornecedor");

            var espec2 = new ProdutoCategoriaEspecificacao(1)
                .Incluir(p => p.Fornecedor);

            // Act
            var combinada = espec1.E(espec2);

            // Assert
            Assert.Contains(combinada.Inclusoes, e => e.ToString().Contains("Categoria"));
            Assert.Contains(combinada.Inclusoes, e => e.ToString().Contains("Fornecedor"));
            Assert.Equal(2, combinada.Inclusoes.Count);
            Assert.Single(combinada.InclusoesPorString);
        }

        [Fact]
        public void Deve_RemoverInclusoesDuplicadas_Quando_CombinarEspecificacoes()
        {
            // Arrange
            var espec1 = new ProdutoAtivoEspecificacao()
                .Incluir(p => p.Categoria);

            var espec2 = new ProdutoCategoriaEspecificacao(1)
                .Incluir(p => p.Categoria); // Duplicada

            // Act
            var combinada = espec1.E(espec2);

            // Assert
            Assert.Single(combinada.Inclusoes);
        }

        [Fact]
        public void Deve_RemoverInclusoesPorStringDuplicadas_Quando_CombinarEspecificacoes()
        {
            // Arrange
            var espec1 = new ProdutoAtivoEspecificacao()
                .Incluir("Categoria");

            var espec2 = new ProdutoCategoriaEspecificacao(1)
                .Incluir("Categoria"); // Duplicada

            // Act
            var combinada = espec1.E(espec2);

            // Assert
            Assert.Single(combinada.InclusoesPorString);
        }

        [Fact]
        public void Deve_LancarExcecao_Quando_CombinarComNulo()
        {
            // Arrange
            var espec = new ProdutoAtivoEspecificacao();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => espec.E(null));
            Assert.Throws<ArgumentNullException>(() => espec.Ou(null));
            Assert.Throws<ArgumentNullException>(() => ((IEspecificacao<Produto>)null).E(espec));
            Assert.Throws<ArgumentNullException>(() => ((IEspecificacao<Produto>)null).Ou(espec));
        }

        [Fact]
        public void Deve_LancarExcecao_Quando_AdicionarInclusaoEmEspecificacaoCombinada()
        {
            // Arrange
            var espec1 = new ProdutoAtivoEspecificacao();
            var espec2 = new ProdutoCategoriaEspecificacao(1);
            var combinada = espec1.E(espec2);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => combinada.Incluir(p => p.Categoria));
            Assert.Throws<NotSupportedException>(() => combinada.Incluir("Categoria"));
        }
    }
}
