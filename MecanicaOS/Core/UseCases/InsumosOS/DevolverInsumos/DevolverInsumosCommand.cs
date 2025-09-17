using Core.Entidades;

namespace Core.UseCases.InsumosOS.DevolverInsumos
{
    public class DevolverInsumosCommand
    {
        public IEnumerable<InsumoOS> InsumosOS { get; set; }

        public DevolverInsumosCommand(IEnumerable<InsumoOS> insumosOS)
        {
            InsumosOS = insumosOS ?? throw new ArgumentNullException(nameof(insumosOS));
        }
    }
}
