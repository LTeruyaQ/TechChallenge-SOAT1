using Core.UseCases.Servicos.ObterServicosDisponiveis;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Servicos
{
    /// <summary>
    /// Interface para o handler de obtenção de serviços disponíveis
    /// </summary>
    public interface IObterServicosDisponiveisHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de serviços disponíveis
        /// </summary>
        /// <returns>Resposta contendo a lista de serviços disponíveis</returns>
        Task<ObterServicosDisponiveisResponse> Handle();
    }
}
