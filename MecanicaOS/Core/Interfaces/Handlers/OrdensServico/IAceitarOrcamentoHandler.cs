using Core.UseCases.OrdensServico.AceitarOrcamento;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.OrdensServico
{
    /// <summary>
    /// Interface para o handler de aceitação de orçamento
    /// </summary>
    public interface IAceitarOrcamentoHandler
    {
        /// <summary>
        /// Manipula a operação de aceitação de orçamento
        /// </summary>
        /// <param name="id">ID da ordem de serviço cujo orçamento será aceito</param>
        /// <returns>Resposta indicando o sucesso da operação</returns>
        Task<AceitarOrcamentoResponse> Handle(Guid id);
    }
}
