namespace Aplicacao.UseCases.Estoque.AtualizarEstoque
{
    public interface IAtualizarEstoqueUseCase
    {
        Task<AtualizarEstoqueResponse> ExecuteAsync(Guid id, AtualizarEstoqueRequest request);
    }
}