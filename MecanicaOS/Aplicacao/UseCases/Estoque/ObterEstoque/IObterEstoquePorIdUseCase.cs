namespace Aplicacao.UseCases.Estoque.ObterEstoque
{
    public interface IObterEstoquePorIdUseCase
    {
        Task<EstoqueResponse> ExecuteAsync(Guid id);
    }
}