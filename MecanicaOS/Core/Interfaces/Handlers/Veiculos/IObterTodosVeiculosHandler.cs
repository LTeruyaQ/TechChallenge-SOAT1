using Core.UseCases.Veiculos.ObterTodosVeiculos;

namespace Core.Interfaces.Handlers.Veiculos
{
    /// <summary>
    /// Interface para o handler de obtenção de todos os veículos
    /// </summary>
    public interface IObterTodosVeiculosHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de todos os veículos
        /// </summary>
        /// <returns>Resposta contendo a lista de veículos</returns>
        Task<ObterTodosVeiculosResponse> Handle();
    }
}
