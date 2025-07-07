using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Estoque;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IClienteServico
    {
        Task<ClienteResponse> CadastrarAsync(CadastrarClienteRequest request);
        Task<ClienteResponse> AtualizarAsync(Guid id, AtualizarClienteRequest request);
        Task<bool> DeletarAsync(Guid id);
        Task<ClienteResponse> ObterPorIdAsync(Guid id);
        Task<IEnumerable<ClienteResponse>> ObterTodosAsync();
    }
}
