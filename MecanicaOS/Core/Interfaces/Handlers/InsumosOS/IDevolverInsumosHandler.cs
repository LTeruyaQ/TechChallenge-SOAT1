using Core.DTOs.Requests.OrdemServico.InsumoOS;

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
        /// <returns>Indica se a operação foi bem-sucedida</returns>
        Task<bool> Handle(IEnumerable<DevolverInsumoOSRequest> insumosOS);
    }
}
