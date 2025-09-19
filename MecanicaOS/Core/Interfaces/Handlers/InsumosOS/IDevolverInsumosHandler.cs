using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.UseCases.InsumosOS.DevolverInsumos;

namespace Core.Interfaces.Handlers.InsumosOS
{
    /// <summary>
    /// Interface para o handler de devolução de insumos ao estoque
    /// </summary>
    public interface IDevolverInsumosHandler
    {
        /// <summary>
        /// Manipula a operação de devolução de insumos ao estoque
        /// </summary>
        /// <param name="insumosOS">Lista de insumos a serem devolvidos</param>
        /// <returns>Resposta indicando o sucesso da operação</returns>
        Task<DevolverInsumosResponse> Handle(IEnumerable<DevolverInsumoOSRequest> insumosOS);
    }
}
