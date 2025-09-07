using Aplicacao.DTOs.Responses.Estoque;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IEstoqueServico
    {
        Task<EstoqueResponse> ObterPorIdAsync(Guid id);
        Task<IEnumerable<EstoqueResponse>> ObterTodosAsync();
    }
}