using Core.Entidades;

namespace Core.UseCases.OrdensServico.ObterTodosOrdensServico
{
    public class ObterTodosOrdensServicoResponse
    {
        public IEnumerable<OrdemServico> OrdensServico { get; set; } = Enumerable.Empty<OrdemServico>();
    }
}
