using Core.Entidades;

namespace Core.UseCases.Estoques.ObterTodosEstoques
{
    public class ObterTodosEstoquesResponse
    {
        public IEnumerable<Estoque> Estoques { get; set; } = null!;
    }
}
