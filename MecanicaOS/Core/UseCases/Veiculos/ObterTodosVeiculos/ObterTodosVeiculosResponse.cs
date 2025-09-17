using Core.Entidades;

namespace Core.UseCases.Veiculos.ObterTodosVeiculos
{
    public class ObterTodosVeiculosResponse
    {
        public IEnumerable<Veiculo> Veiculos { get; set; } = Enumerable.Empty<Veiculo>();
    }
}
