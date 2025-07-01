using Dominio.DTOs;
using Dominio.Entidades;

namespace Dominio.Interfaces.Services
{
    public interface IServicoServico
    {
        Task<Servico> ObterServicoPorId(Guid id);
        Task<IEnumerable<Servico>> ObterTodos();
        Task<IEnumerable<Servico>> ObterServicosPorFiltro(FiltrarServicoDto filtro);
        Task CadastrarServico(Servico servico);
        Task EditarServico(Guid id, Servico servico);
        Task DeletarServico(Guid id);
    }
}
