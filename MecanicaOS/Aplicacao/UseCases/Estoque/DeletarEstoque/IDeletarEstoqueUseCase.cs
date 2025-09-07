namespace Aplicacao.UseCases.Estoque.DeletarEstoque
{
    public interface IDeletarEstoqueUseCase
    {
        Task<bool> ExecuteAsync(Guid id);
    }
}
