namespace Aplicacao.UseCases.Estoque.ListaEstoque
{
    public interface IListarEstoqueUseCase
    {
        Task<IEnumerable<Dominio.Entidades.Estoque>> ExecutarAsync();
    }
}