using Core.Entidades;
using Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Gateways
{
    public interface IOrdemServicoGateway
    {
        Task<OrdemServico> CadastrarAsync(OrdemServico ordemServico);
        Task EditarAsync(OrdemServico ordemServico);
        Task EditarVariosAsync(IEnumerable<OrdemServico> ordensServico);
        Task<IEnumerable<OrdemServico>> ListarOSOrcamentoExpiradoAsync();
        Task<OrdemServico?> ObterOrdemServicoPorIdComInsumos(Guid id);
        Task<IEnumerable<OrdemServico>> ObterOrdemServicoPorStatusAsync(StatusOrdemServico status);
        Task<OrdemServico?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<OrdemServico>> ObterTodosAsync();
    }
}
