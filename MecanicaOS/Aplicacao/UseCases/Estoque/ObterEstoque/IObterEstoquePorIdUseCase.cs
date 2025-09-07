namespace Aplicacao.UseCases.Estoque.ObterEstoque
{
    public interface IObterEstoquePorIdUseCase
    {
        Task<Dominio.Entidades.Estoque> ExecutarAsync(Guid id);
    }
}