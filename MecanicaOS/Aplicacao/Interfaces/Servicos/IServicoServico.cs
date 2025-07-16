using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IServicoServico
    {
        Task<ServicoResponse> ObterServicoPorIdAsync(Guid id);
        Task<IEnumerable<ServicoResponse>> ObterTodosAsync();
        Task<IEnumerable<ServicoResponse>> ObterServicosDisponiveisAsync();
        Task<ServicoResponse?> ObterServicoPorNomeAsync(string nome);
        Task<ServicoResponse> CadastrarServicoAsync(CadastrarServicoRequest request);
        Task<ServicoResponse> EditarServicoAsync(Guid id, EditarServicoRequest request);
        Task DeletarServicoAsync(Guid id);
    }
}
