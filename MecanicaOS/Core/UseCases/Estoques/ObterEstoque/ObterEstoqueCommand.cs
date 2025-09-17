namespace Core.UseCases.Estoques.ObterEstoque
{
    public class ObterEstoqueCommand
    {
        public Guid Id { get; set; }

        public ObterEstoqueCommand(Guid id)
        {
            Id = id;
        }
    }
}
