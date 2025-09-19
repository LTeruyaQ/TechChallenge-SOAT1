using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Entidades;
using Core.Interfaces.Presenters;

namespace Adapters.Presenters
{
    public class InsumoOSPresenter : IInsumoPresenter
    {
        public IEnumerable<InsumoOSResponse> ToResponse(IEnumerable<InsumoOS> insumos)
        {
            if (insumos == null)
                return [];

            return insumos.Select(insumo => new InsumoOSResponse
            {
                EstoqueId = insumo.EstoqueId,
                OrdemServicoId = insumo.OrdemServicoId,
                Quantidade = insumo.Quantidade
            });
        }
    }
}
