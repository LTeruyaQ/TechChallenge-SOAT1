using Core.UseCases.Clientes.ObterCliente;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Clientes
{
    /// <summary>
    /// Interface para o handler de obtenção de cliente por ID
    /// </summary>
    public interface IObterClienteHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de cliente por ID
        /// </summary>
        /// <param name="id">ID do cliente a ser obtido</param>
        /// <returns>Resposta contendo o cliente encontrado</returns>
        Task<ObterClienteResponse> Handle(Guid id);
    }
}
