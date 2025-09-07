namespace Aplicacao.UseCases.Estoque.CriarEstoque
{
    public interface ICriarEstoqueUseCase
    {
        Task<CriarEstoqueResponse> ExecuteAsync(CriarEstoqueRequest request);
    }
}