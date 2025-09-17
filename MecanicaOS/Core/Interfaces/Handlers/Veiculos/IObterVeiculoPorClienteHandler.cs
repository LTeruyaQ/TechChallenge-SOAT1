using Core.UseCases.Veiculos.ObterVeiculoPorCliente;

namespace Core.Interfaces.Handlers.Veiculos
{
    /// <summary>
    /// Interface para o handler de obtenção de veículos por cliente
    /// </summary>
    public interface IObterVeiculoPorClienteHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de veículos por cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Resposta contendo a lista de veículos do cliente</returns>
        Task<ObterVeiculoPorClienteResponse> Handle(Guid clienteId);
    }
}
