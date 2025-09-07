namespace Aplicacao.UseCases.Estoque.CriarEstoque
{
    public interface ICriarEstoqueUseCase
    {
        Task<Dominio.Entidades.Estoque> ExecutarAsync(CriarEstoqueRequest request);
    }
}