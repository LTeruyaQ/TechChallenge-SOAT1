using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IEstoqueServico
    {
        Task<EstoqueResponse> ObterPorIdAsync(Guid id);
        Task<EstoqueResponse> CadastrarAsync(CadastrarEstoqueRequest request);
        Task<EstoqueResponse> AtualizarAsync(Guid id, AtualizarEstoqueRequest request);
        Task<bool> DeletarAsync(Guid id);
        Task<IEnumerable<EstoqueResponse>> ObterTodosAsync();
    }
}
