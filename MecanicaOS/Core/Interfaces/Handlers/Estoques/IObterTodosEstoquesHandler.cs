using Core.UseCases.Estoques.ObterTodosEstoques;

namespace Core.Interfaces.Handlers.Estoques
{
    /// <summary>
    /// Interface para o handler de obtenção de todos os estoques
    /// </summary>
    public interface IObterTodosEstoquesHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de todos os estoques
        /// </summary>
        /// <returns>Resposta contendo a lista de estoques</returns>
        Task<ObterTodosEstoquesResponse> Handle();
    }
}
