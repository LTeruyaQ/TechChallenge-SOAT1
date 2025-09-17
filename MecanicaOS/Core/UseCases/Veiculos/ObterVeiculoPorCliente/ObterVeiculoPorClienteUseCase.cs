namespace Core.UseCases.Veiculos.ObterVeiculoPorCliente
{
    public class ObterVeiculoPorClienteUseCase
    {
        public Guid ClienteId { get; set; }

        public ObterVeiculoPorClienteUseCase(Guid clienteId)
        {
            ClienteId = clienteId;
        }
    }
}
