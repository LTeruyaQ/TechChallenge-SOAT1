using Core.Entidades;

namespace Core.UseCases.Veiculos.ObterVeiculoPorCliente
{
    public class ObterVeiculoPorClienteResponse
    {
        public IEnumerable<Veiculo> Veiculos { get; set; } = Enumerable.Empty<Veiculo>();
    }
}
