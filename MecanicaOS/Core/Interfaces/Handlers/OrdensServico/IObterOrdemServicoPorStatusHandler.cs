using Core.Enumeradores;
using Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.OrdensServico
{
    /// <summary>
    /// Interface para o handler de obtenção de ordens de serviço por status
    /// </summary>
    public interface IObterOrdemServicoPorStatusHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de ordens de serviço por status
        /// </summary>
        /// <param name="status">Status das ordens de serviço a serem obtidas</param>
        /// <returns>Resposta contendo a lista de ordens de serviço com o status especificado</returns>
        Task<ObterOrdemServicoPorStatusResponse> Handle(StatusOrdemServico status);
    }
}
