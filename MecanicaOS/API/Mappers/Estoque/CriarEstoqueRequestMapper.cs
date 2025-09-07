using API.DTOs.Request.Estoque;

namespace API.Mappers.Estoque
{
    public static class CriarEstoqueRequestMapper
    {
        public static Dominio.Entidades.Estoque ParaEntidade(CriarEstoqueRequest request)
        {
            return new Dominio.Entidades.Estoque
            (
                request.Insumo,
                request.Descricao,
                request.Preco,
                request.QuantidadeDisponivel,
                request.QuantidadeMinima
            );
        }
    }
}