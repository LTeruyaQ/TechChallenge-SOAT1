namespace Core.UseCases.Clientes.RemoverCliente
{
    public class RemoverClienteCommand
    {
        public Guid Id { get; set; }

        public RemoverClienteCommand(Guid id)
        {
            Id = id;
        }
    }
}
