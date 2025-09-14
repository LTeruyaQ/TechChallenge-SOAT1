using Adapters.DTOs.Requests.Autenticacao;
using Adapters.DTOs.Responses.Autenticacao;
using Core.DTOs.UseCases.Autenticacao;

namespace Adapters.Presenters.Interfaces
{
    public interface IAutenticacaoPresenter
    {
        AutenticacaoUseCaseDto? ParaUseCaseDto(AutenticacaoRequest request);
        AutenticacaoResponse? ParaResponse(AutenticacaoDto autenticacaoUseCaseDto);
    }
}
