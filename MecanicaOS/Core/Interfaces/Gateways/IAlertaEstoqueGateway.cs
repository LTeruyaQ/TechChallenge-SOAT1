using Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Gateways
{
    public interface IAlertaEstoqueGateway
    {
        Task CadastrarVariosAsync(IEnumerable<AlertaEstoque> alertas);
        Task<IEnumerable<AlertaEstoque>> ObterAlertaDoDiaPorEstoqueAsync(Guid insumoId, DateTime dataAtual);
    }
}
