namespace Aplicacao.UseCases.Estoque.DeletarEstoque
{
    public interface IDeletarEstoqueUseCase
    {
        Task<bool> ExecutarAsync(Guid id);
    }
}
