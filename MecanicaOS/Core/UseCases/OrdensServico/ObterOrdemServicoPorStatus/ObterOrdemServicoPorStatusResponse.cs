using Core.Entidades;

namespace Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus
{
    public class ObterOrdemServicoPorStatusResponse
    {
        public IEnumerable<OrdemServico> OrdensServico { get; set; } = Enumerable.Empty<OrdemServico>();
    }
}
