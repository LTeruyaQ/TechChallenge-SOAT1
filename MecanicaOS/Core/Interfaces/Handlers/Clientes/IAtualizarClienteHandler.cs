using Core.DTOs.UseCases.Cliente;
using Core.UseCases.Clientes.AtualizarCliente;

namespace Core.Interfaces.Handlers.Clientes
{
    /// <summary>
    /// Interface para o handler de atualização de cliente
    /// </summary>
    public interface IAtualizarClienteHandler
    {
        /// <summary>
        /// Manipula a operação de atualização de cliente
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="request">DTO com os dados atualizados do cliente</param>
        /// <returns>Resposta contendo o cliente atualizado</returns>
        Task<AtualizarClienteResponse> Handle(Guid id, AtualizarClienteUseCaseDto request);
    }
}
