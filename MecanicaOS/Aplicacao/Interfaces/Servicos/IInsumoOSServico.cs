using Aplicacao.DTOs.Requests.OrdemServico.InsumoOrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;

namespace Aplicacao.Interfaces.Servicos;

public interface IInsumoOSServico
{
    Task<List<InsumoOSResponse>> CadastrarInsumosAsync(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request);
}