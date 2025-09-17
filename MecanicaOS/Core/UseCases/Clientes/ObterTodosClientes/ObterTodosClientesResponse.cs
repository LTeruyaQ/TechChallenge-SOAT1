using Core.Entidades;

namespace Core.UseCases.Clientes.ObterTodosClientes
{
    public class ObterTodosClientesResponse
    {
        public IEnumerable<Cliente> Clientes { get; set; } = Enumerable.Empty<Cliente>();
    }
}
