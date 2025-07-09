using Dominio.Especificacoes.Base;
using System.Linq.Expressions;
using Testes.Especificacoes.Base.Modelos;

namespace Testes.Especificacoes.Base
{
    public abstract class EspecificacaoTestBase
    {
        protected class ProdutoAtivoEspecificacao : Especificacao<Produto>
        {
            public override Expression<Func<Produto, bool>> Expressao => p => p.Ativo;
        }

        protected class ProdutoCategoriaEspecificacao : Especificacao<Produto>
        {
            private readonly int _categoriaId;

            public ProdutoCategoriaEspecificacao(int categoriaId)
            {
                _categoriaId = categoriaId;
            }

            public override Expression<Func<Produto, bool>> Expressao =>
                p => p.CategoriaId == _categoriaId;
        }

        protected class ProdutoPrecoMinimoEspecificacao : Especificacao<Produto>
        {
            private readonly decimal _precoMinimo;

            public ProdutoPrecoMinimoEspecificacao(decimal precoMinimo)
            {
                _precoMinimo = precoMinimo;
            }

            public override Expression<Func<Produto, bool>> Expressao =>
                p => p.Preco >= _precoMinimo;
        }

        protected IQueryable<Produto> CriarProdutosParaTeste()
        {
            var categoria1 = new Categoria { Id = 1, Nome = "Eletrônicos" };
            var categoria2 = new Categoria { Id = 2, Nome = "Livros" };
            var fornecedor = new Fornecedor { Id = 1, Nome = "Fornecedor Teste" };

            return new List<Produto>
            {
                new() { Id = 1, Nome = "Notebook", Preco = 3500, Ativo = true, Categoria = categoria1, CategoriaId = 1, Fornecedor = fornecedor },
                new() { Id = 2, Nome = "Smartphone", Preco = 2000, Ativo = true, Categoria = categoria1, CategoriaId = 1, Fornecedor = fornecedor },
                new() { Id = 3, Nome = "Livro C#", Preco = 120, Ativo = true, Categoria = categoria2, CategoriaId = 2, Fornecedor = fornecedor },
                new() { Id = 4, Nome = "Tablet", Preco = 1500, Ativo = false, Categoria = categoria1, CategoriaId = 1, Fornecedor = fornecedor },
                new() { Id = 5, Nome = "Livro EF Core", Preco = 250, Ativo = true, Categoria = categoria2, CategoriaId = 2, Fornecedor = fornecedor }
            }.AsQueryable();
        }
    }
}
