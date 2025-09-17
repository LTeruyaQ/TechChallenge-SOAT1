namespace Core.UseCases.OrdensServico.AceitarOrcamento
{
    public class AceitarOrcamentoCommand
    {
        public Guid Id { get; set; }

        public AceitarOrcamentoCommand(Guid id)
        {
            Id = id;
        }
    }
}
