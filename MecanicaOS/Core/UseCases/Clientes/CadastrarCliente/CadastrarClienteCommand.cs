using Core.DTOs.UseCases.Cliente;

namespace Core.UseCases.Clientes.CadastrarCliente
{
    public class CadastrarClienteCommand
    {
        public CadastrarClienteUseCaseDto Request { get; set; }

        public CadastrarClienteCommand(CadastrarClienteUseCaseDto request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
