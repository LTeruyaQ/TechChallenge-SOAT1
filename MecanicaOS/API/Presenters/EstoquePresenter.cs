using Aplicacao.UseCases.Estoque;
using Dominio.Entidades;

namespace API.Presenters
{
    public class EstoquePresenter
    {
        public static EstoqueResponse ParaResponse(Estoque estoque)
        {
            return new EstoqueResponse
            {
                Id = estoque.Id,
                Insumo = estoque.Insumo,
                Descricao = estoque.Descricao,
                Preco = estoque.Preco,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                QuantidadeMinima = estoque.QuantidadeMinima,
                DataCadastro = estoque.DataCadastro,
                DataAtualizacao = estoque.DataAtualizacao
            };
        }

        public static IEnumerable<EstoqueResponse> ParaIEnumerableResponse(IEnumerable<Estoque> estoques)
        {
            return estoques.Select(e => ParaResponse(e));
        }
    }
}
