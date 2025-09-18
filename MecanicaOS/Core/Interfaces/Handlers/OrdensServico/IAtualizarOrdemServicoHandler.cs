using Core.DTOs.UseCases.OrdemServico;
using Core.UseCases.OrdensServico.AtualizarOrdemServico;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.OrdensServico
{
    /// <summary>
    /// Interface para o handler de atualização de ordens de serviço
    /// </summary>
    public interface IAtualizarOrdemServicoHandler
    {
        /// <summary>
        /// Manipula a operação de atualização de ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço a ser atualizada</param>
        /// <param name="request">DTO com os dados atualizados da ordem de serviço</param>
        /// <returns>Resposta contendo a ordem de serviço atualizada</returns>
        Task<AtualizarOrdemServicoResponse> Handle(Guid id, AtualizarOrdemServicoUseCaseDto request);
    }
}
