using Dominio.Entidades;

namespace Dominio.Interfaces.Services
{
    public interface IServicoServico
    {
        Task<Servico> ObterServicoPorId(Guid id);
        Task<IEnumerable<Servico>> ObterServicosPorFiltro(Servico filtro);
        Task CadastrarServico(Servico servico);
        Task EditarServico(Guid id, Servico servico);
        Task DeletarServico(Guid id);
    }
}
