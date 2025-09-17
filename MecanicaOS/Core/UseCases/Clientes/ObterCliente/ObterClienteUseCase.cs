namespace Core.UseCases.Clientes.ObterCliente
{
    public class ObterClienteUseCase
    {
        public Guid Id { get; set; }

        public ObterClienteUseCase(Guid id)
        {
            Id = id;
        }
    }
}
