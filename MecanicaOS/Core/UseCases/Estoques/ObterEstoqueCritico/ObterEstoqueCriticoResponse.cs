using Core.Entidades;

namespace Core.UseCases.Estoques.ObterEstoqueCritico
{
    public class ObterEstoqueCriticoResponse
    {
        public IEnumerable<Estoque> EstoquesCriticos { get; set; } = null!;
    }
}
