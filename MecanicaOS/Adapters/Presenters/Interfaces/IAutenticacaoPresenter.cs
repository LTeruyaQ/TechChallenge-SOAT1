using Adapters.DTOs.Requests.Autenticacao;
using Adapters.DTOs.Responses.Autenticacao;
using Core.DTOs.Autenticacao;

namespace Adapters.Presenters.Interfaces
{
    public interface IAutenticacaoPresenter
    {
        AutenticacaoUseCaseDto? ParaUseCaseDto(AutenticacaoRequest request);
        AutenticacaoResponse? ParaResponse(AutenticacaoDto autenticacaoUseCaseDto);
    }
}
