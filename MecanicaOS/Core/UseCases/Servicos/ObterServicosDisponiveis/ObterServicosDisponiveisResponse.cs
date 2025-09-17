using Core.Entidades;

namespace Core.UseCases.Servicos.ObterServicosDisponiveis
{
    public class ObterServicosDisponiveisResponse
    {
        public IEnumerable<Servico> Servicos { get; set; } = Enumerable.Empty<Servico>();
    }
}
