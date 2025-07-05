using Dominio.DTOs.Servico;
using Dominio.Entidades;

namespace Dominio.Interfaces.Servicos
{
    public interface IServicoServico
    {
        Task<Servico> ObterServicoPorIdAsync(Guid id);
        Task<IEnumerable<Servico>> ObterTodosAsync();
        Task<IEnumerable<Servico>> ObterServicosPorFiltroAsync(FiltrarServicoDto filtro);
        Task<Servico> CadastrarServicoAsync(CadastrarServicoDto servico);
        Task EditarServicoAsync(Guid id, EditarServicoDto servico);
        Task DeletarServicoAsync(Guid id);
    }
}
