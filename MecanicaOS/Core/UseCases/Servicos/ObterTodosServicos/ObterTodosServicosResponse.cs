using Core.Entidades;

namespace Core.UseCases.Servicos.ObterTodosServicos
{
    public class ObterTodosServicosResponse
    {
        public IEnumerable<Servico> Servicos { get; set; } = Enumerable.Empty<Servico>();
    }
}
