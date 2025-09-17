namespace Core.UseCases.OrdensServico.RecusarOrcamento
{
    public class RecusarOrcamentoCommand
    {
        public Guid Id { get; set; }

        public RecusarOrcamentoCommand(Guid id)
        {
            Id = id;
        }
    }
}
