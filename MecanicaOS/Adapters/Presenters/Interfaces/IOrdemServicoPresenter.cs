using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.OrdemServico;
using Core.DTOs.OrdemServico.InsumoOS;

namespace Adapters.Presenters.Interfaces
{
    public interface IOrdemServicoPresenter
    {
        CadastrarOrdemServicoUseCaseDto ParaUseCaseDto(CadastrarOrdemServicoRequest request);
        AtualizarOrdemServicoUseCaseDto ParaUseCaseDto(AtualizarOrdemServicoRequest request);
        CadastrarInsumoOSUseCaseDto ParaUseCaseDto(CadastrarInsumoOSRequest request);
    }
}
