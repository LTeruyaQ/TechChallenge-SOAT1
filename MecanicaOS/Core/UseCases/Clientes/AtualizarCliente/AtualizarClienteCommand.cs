using Core.DTOs.UseCases.Cliente;

namespace Core.UseCases.Clientes.AtualizarCliente
{
    public class AtualizarClienteCommand
    {
        public Guid Id { get; set; }
        public AtualizarClienteUseCaseDto Request { get; set; }

        public AtualizarClienteCommand(Guid id, AtualizarClienteUseCaseDto request)
        {
            Id = id;
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
