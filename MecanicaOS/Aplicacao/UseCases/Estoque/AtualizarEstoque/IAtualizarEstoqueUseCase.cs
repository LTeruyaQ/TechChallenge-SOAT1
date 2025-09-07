namespace Aplicacao.UseCases.Estoque.AtualizarEstoque
{
    public interface IAtualizarEstoqueUseCase
    {
        Task<Dominio.Entidades.Estoque> ExecutarAsync(Dominio.Entidades.Estoque request);
    }
}