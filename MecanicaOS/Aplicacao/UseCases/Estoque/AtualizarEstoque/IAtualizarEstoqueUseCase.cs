namespace Aplicacao.UseCases.Estoque.AtualizarEstoque
{
    public interface IAtualizarEstoqueUseCase
    {
        Task<EstoqueResponse> ExecuteAsync(Guid id, AtualizarEstoqueRequest request);
    }
}