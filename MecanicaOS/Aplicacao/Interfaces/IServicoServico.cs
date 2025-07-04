using Aplicacao.DTOs.Servico;
using Dominio.DTOs;
using Dominio.Entidades;

namespace Dominio.Interfaces.Services
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
