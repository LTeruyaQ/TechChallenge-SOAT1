using Core.UseCases.Servicos.ObterTodosServicos;

namespace Core.Interfaces.Handlers.Servicos
{
    /// <summary>
    /// Interface para o handler de obtenção de todos os serviços
    /// </summary>
    public interface IObterTodosServicosHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de todos os serviços
        /// </summary>
        /// <returns>Resposta contendo a lista de serviços</returns>
        Task<ObterTodosServicosResponse> Handle();
    }
}
