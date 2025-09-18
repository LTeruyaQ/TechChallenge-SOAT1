using Core.UseCases.Estoques.ObterEstoqueCritico;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Estoques
{
    /// <summary>
    /// Interface para o handler de obtenção de estoques críticos
    /// </summary>
    public interface IObterEstoqueCriticoHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de estoques críticos
        /// </summary>
        /// <returns>Resposta contendo a lista de estoques críticos</returns>
        Task<ObterEstoqueCriticoResponse> Handle();
    }
}
