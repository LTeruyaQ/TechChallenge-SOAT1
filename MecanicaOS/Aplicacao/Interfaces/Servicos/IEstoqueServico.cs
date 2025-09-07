using Aplicacao.UseCases.Estoque;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IEstoqueServico
    {
        Task<IEnumerable<EstoqueResponse>> ObterTodosAsync();
    }
}