using Core.UseCases.Clientes.RemoverCliente;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Clientes
{
    /// <summary>
    /// Interface para o handler de remoção de cliente
    /// </summary>
    public interface IRemoverClienteHandler
    {
        /// <summary>
        /// Manipula a operação de remoção de cliente
        /// </summary>
        /// <param name="id">ID do cliente a ser removido</param>
        /// <returns>Resposta indicando o sucesso da operação</returns>
        Task<RemoverClienteResponse> Handle(Guid id);
    }
}
