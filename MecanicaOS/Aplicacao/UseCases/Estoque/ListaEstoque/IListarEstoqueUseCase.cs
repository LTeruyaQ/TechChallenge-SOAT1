namespace Aplicacao.UseCases.Estoque.ListaEstoque
{
    public interface IListarEstoqueUseCase
    {
        Task<List<EstoqueResponse>> ExecuteAsync();
    }
}