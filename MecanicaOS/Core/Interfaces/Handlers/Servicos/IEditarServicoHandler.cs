using Core.DTOs.UseCases.Servico;
using Core.UseCases.Servicos.EditarServico;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Servicos
{
    /// <summary>
    /// Interface para o handler de edição de serviços
    /// </summary>
    public interface IEditarServicoHandler
    {
        /// <summary>
        /// Manipula a operação de edição de serviço
        /// </summary>
        /// <param name="id">ID do serviço a ser editado</param>
        /// <param name="request">DTO com os dados atualizados do serviço</param>
        /// <returns>Resposta contendo o serviço atualizado</returns>
        Task<EditarServicoResponse> Handle(Guid id, EditarServicoUseCaseDto request);
    }
}
