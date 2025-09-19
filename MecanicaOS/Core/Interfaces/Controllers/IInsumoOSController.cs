using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.Entidades;

namespace Core.Interfaces.Controllers
{
    public interface IInsumoOSController
    {
        Task<IEnumerable<InsumoOS>> CadastrarInsumos(Guid ordemServicoId, List<CadastrarInsumoOSRequest> requests);
        Task DevolverInsumosAoEstoque(IEnumerable<InsumoOS> insumosOS);
    }
}