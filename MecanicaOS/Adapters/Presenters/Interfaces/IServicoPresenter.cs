using Adapters.DTOs.Requests.Servico;
using Adapters.DTOs.Responses.Servico;
using Core.DTOs.Servico;
using Core.Entidades;

namespace Adapters.Presenters.Interfaces
{
    public interface IServicoPresenter
    {
        IEnumerable<ServicoResponse?> ParaResponse(IEnumerable<Servico> enumerable);
        ServicoResponse? ParaResponse(Servico servico);
        CadastrarServicoUseCaseDto? ParaUseCaseDto(CadastrarServicoRequest request);
        EditarServicoUseCaseDto? ParaUseCaseDto(EditarServicoRequest request);
    }
}
