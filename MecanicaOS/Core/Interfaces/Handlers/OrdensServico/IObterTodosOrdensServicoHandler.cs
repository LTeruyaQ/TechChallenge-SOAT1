using Core.UseCases.OrdensServico.ObterTodosOrdensServico;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.OrdensServico
{
    /// <summary>
    /// Interface para o handler de obtenção de todas as ordens de serviço
    /// </summary>
    public interface IObterTodosOrdensServicoHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de todas as ordens de serviço
        /// </summary>
        /// <returns>Resposta contendo a lista de ordens de serviço</returns>
        Task<ObterTodosOrdensServicoResponse> Handle();
    }
}
