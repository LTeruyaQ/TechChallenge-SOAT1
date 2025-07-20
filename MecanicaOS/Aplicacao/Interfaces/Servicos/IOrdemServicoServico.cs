using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico;
using Dominio.Enumeradores;

namespace Aplicacao.Interfaces.Servicos;

public interface IOrdemServicoServico
{
    Task<OrdemServicoResponse?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<OrdemServicoResponse>> ObterPorStatusAsync(StatusOrdemServico status);
    Task<OrdemServicoResponse> CadastrarAsync(CadastrarOrdemServicoRequest request);
    Task<OrdemServicoResponse> AtualizarAsync(Guid id, AtualizarOrdemServicoRequest request);
    Task<IEnumerable<OrdemServicoResponse>> ObterTodosAsync();
    Task AceitarOrcamentoAsync(Guid id);
    Task RecusarOrcamentoAsync(Guid id);
}