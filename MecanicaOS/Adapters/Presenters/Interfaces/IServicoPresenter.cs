using Aplicacao.DTOs.Requests.Servico;
using Core.DTOs.Servico;

namespace Adapters.Presenters.Interfaces
{
    public interface IServicoPresenter
    {
        CadastrarServicoUseCaseDto ParaUseCaseDto(CadastrarServicoRequest request);
        EditarServicoUseCaseDto ParaUseCaseDto(EditarServicoRequest request);
    }
}
