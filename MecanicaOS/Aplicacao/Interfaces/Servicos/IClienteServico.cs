using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses.Cliente;
using Dominio.Entidades;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IClienteServico
    {
        Task<ClienteResponse> CadastrarAsync(CadastrarClienteRequest request);
        Task<ClienteResponse> AtualizarAsync(Guid id, AtualizarClienteRequest request);
        Task<bool> DeletarAsync(Guid id);
        Task<ClienteResponse> ObterPorIdAsync(Guid id);
        Task<IEnumerable<ClienteResponse>> ObterTodosAsync();
        Task<Cliente> ObterPorDocumento(string documento);
    }
}
