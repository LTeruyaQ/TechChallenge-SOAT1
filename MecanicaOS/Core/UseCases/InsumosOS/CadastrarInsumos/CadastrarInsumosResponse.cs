using Core.Entidades;

namespace Core.UseCases.InsumosOS.CadastrarInsumos
{
    public class CadastrarInsumosResponse
    {
        public IEnumerable<InsumoOS> InsumosOS { get; set; } = Enumerable.Empty<InsumoOS>();
    }
}
