using Adapters.DTOs.Requests.OrdemServico;
using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.DTOs.Responses.OrdemServico;
using Core.DTOs.OrdemServico;
using Core.DTOs.OrdemServico.InsumoOS;
using Core.Entidades;

namespace Adapters.Presenters.Interfaces
{
    public interface IOrdemServicoPresenter
    {
        CadastrarOrdemServicoUseCaseDto? ParaUseCaseDto(CadastrarOrdemServicoRequest request);
        AtualizarOrdemServicoUseCaseDto? ParaUseCaseDto(AtualizarOrdemServicoRequest request);
        CadastrarInsumoOSUseCaseDto? ParaUseCaseDto(CadastrarInsumoOSRequest request);
        OrdemServicoResponse? ParaResponse(OrdemServico ordemServico);
        IEnumerable<OrdemServicoResponse?> ParaResponse(IEnumerable<OrdemServico> ordensServico);
    }
}
