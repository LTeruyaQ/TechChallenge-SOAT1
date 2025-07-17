using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Requests.OrdermServico.InsumoOrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;

namespace Aplicacao.Interfaces.Servicos;

public interface IInsumoOSServico
{
    Task<List<InsumoOSResponse>> CadastrarInsumosAsync(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request);
    Task<List<InsumoOSResponse>> AtualizarInsumosAsync(Guid ordemServicoId, List<AtualizarInsumoOSRequest> request);
    Task ApagarInsumosOS(Guid ordemServicoId, List<Guid> insumosId);
}
