namespace Aplicacao.UseCases.Estoque.CriarEstoque
{
    public interface ICriarEstoqueUseCase
    {
        Task<EstoqueResponse> ExecuteAsync(CriarEstoqueRequest request);
    }
}