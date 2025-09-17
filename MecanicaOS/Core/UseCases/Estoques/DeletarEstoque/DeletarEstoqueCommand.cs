namespace Core.UseCases.Estoques.DeletarEstoque
{
    public class DeletarEstoqueCommand
    {
        public Guid Id { get; set; }

        public DeletarEstoqueCommand(Guid id)
        {
            Id = id;
        }
    }
}
