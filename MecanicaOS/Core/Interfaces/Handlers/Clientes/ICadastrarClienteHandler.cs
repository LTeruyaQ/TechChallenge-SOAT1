using Core.DTOs.UseCases.Cliente;
using Core.UseCases.Clientes.CadastrarCliente;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Clientes
{
    /// <summary>
    /// Interface para o handler de cadastro de cliente
    /// </summary>
    public interface ICadastrarClienteHandler
    {
        /// <summary>
        /// Manipula a operação de cadastro de cliente
        /// </summary>
        /// <param name="request">DTO com os dados do cliente a ser cadastrado</param>
        /// <returns>Resposta contendo o cliente cadastrado</returns>
        Task<CadastrarClienteResponse> Handle(CadastrarClienteUseCaseDto request);
    }
}
