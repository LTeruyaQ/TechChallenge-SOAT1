using Aplicacao.DTOs.Requests.OrdemServico.InsumoOrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Dominio.Entidades;

namespace Aplicacao.Interfaces.Servicos;

public interface IInsumoOSServico
{
    Task<List<InsumoOSResponse>> CadastrarInsumosAsync(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request);
    Task DevolverInsumosAoEstoqueAsync(IEnumerable<InsumoOS> insumosOS);
}