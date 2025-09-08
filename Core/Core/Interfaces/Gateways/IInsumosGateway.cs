using Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Gateways
{
    public interface IInsumosGateway
    {
        Task<IEnumerable<InsumoOS>> CadastrarVariosAsync(IEnumerable<InsumoOS> insumosOS);
        Task<IEnumerable<InsumoOS>> ObterInsumosOSPorOSAsync(Guid ordemServicoId);
    }
}
