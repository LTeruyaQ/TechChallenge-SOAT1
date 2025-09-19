using Core.UseCases.OrdensServico.ObterOrdemServico;

namespace Core.Interfaces.Handlers.OrdensServico
{
    /// <summary>
    /// Interface para o handler de obtenção de ordem de serviço por ID
    /// </summary>
    public interface IObterOrdemServicoHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de ordem de serviço por ID
        /// </summary>
        /// <param name="id">ID da ordem de serviço a ser obtida</param>
        /// <returns>Resposta contendo a ordem de serviço encontrada</returns>
        Task<ObterOrdemServicoResponse> Handle(Guid id);
    }
}
