using Aplicacao.DTOs.Servico;
using Dominio.DTOs;
using Dominio.Entidades;

namespace Dominio.Interfaces.Services
{
    public interface IServicoServico
    {
        Task<Servico> ObterServicoPorId(Guid id);
        Task<IEnumerable<Servico>> ObterTodos();
        Task<IEnumerable<Servico>> ObterServicosPorFiltro(FiltrarServicoDto filtro);
        Task<Servico> CadastrarServico(CadastrarServicoDto servico);
        Task EditarServico(Guid id, EditarServicoDto servico);
        Task DeletarServico(Guid id);
    }
}
