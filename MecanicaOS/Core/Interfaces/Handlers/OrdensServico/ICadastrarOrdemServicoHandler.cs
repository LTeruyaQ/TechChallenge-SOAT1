using Core.DTOs.UseCases.OrdemServico;
using Core.UseCases.OrdensServico.CadastrarOrdemServico;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.OrdensServico
{
    /// <summary>
    /// Interface para o handler de cadastro de ordens de serviço
    /// </summary>
    public interface ICadastrarOrdemServicoHandler
    {
        /// <summary>
        /// Manipula a operação de cadastro de ordem de serviço
        /// </summary>
        /// <param name="request">DTO com os dados da ordem de serviço a ser cadastrada</param>
        /// <returns>Resposta contendo a ordem de serviço cadastrada</returns>
        Task<CadastrarOrdemServicoResponse> Handle(CadastrarOrdemServicoUseCaseDto request);
    }
}
