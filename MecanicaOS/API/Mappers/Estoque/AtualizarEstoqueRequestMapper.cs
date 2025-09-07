using API.DTOs.Request.Estoque;
using Aplicacao.UseCases.Estoque.AtualizarEstoque;

namespace API.Mappers.Estoque
{
    public static class AtualizarEstoqueRequestMapper
    {
        public static void ParaEntidade(AtualizarEstoqueRequest request, Dominio.Entidades.Estoque estoque)
        {
            estoque.Atualizar(
                request.Insumo,
                request.Descricao,
                request.Preco,
                request.QuantidadeDisponivel,
                request.QuantidadeMinima
            );
        }
    }
}