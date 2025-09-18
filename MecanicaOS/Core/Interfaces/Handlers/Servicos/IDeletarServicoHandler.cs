using Core.UseCases.Servicos.DeletarServico;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Servicos
{
    /// <summary>
    /// Interface para o handler de deleção de serviços
    /// </summary>
    public interface IDeletarServicoHandler
    {
        /// <summary>
        /// Manipula a operação de deleção de serviço
        /// </summary>
        /// <param name="id">ID do serviço a ser deletado</param>
        /// <returns>Resposta indicando o sucesso da operação</returns>
        Task<DeletarServicoResponse> Handle(Guid id);
    }
}
