using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IVeiculoServico
    {
        Task<VeiculoResponse> CadastrarAsync(CadastrarVeiculoRequest request);
        Task<VeiculoResponse> AtualizarAsync(Guid id, AtualizarVeiculoRequest request);
        Task<bool> DeletarAsync(Guid id);
        Task<VeiculoResponse> ObterPorIdAsync(Guid id);
        Task<IEnumerable<VeiculoResponse>> ObterTodosAsync();
        Task<IEnumerable<VeiculoResponse>> ObterPorClienteAsync(Guid clienteId);
    }
}
