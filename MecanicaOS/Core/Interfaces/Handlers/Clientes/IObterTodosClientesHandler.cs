using Core.UseCases.Clientes.ObterTodosClientes;

namespace Core.Interfaces.Handlers.Clientes
{
    /// <summary>
    /// Interface para o handler de obtenção de todos os clientes
    /// </summary>
    public interface IObterTodosClientesHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de todos os clientes
        /// </summary>
        /// <returns>Resposta contendo a lista de clientes</returns>
        Task<ObterTodosClientesResponse> Handle();
    }
}
