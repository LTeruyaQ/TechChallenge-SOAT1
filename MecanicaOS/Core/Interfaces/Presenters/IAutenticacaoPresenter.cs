using Core.DTOs.Requests.Autenticacao;
using Core.DTOs.Responses.Autenticacao;
using Core.DTOs.UseCases.Autenticacao;

namespace Core.Interfaces.Presenters
{
    public interface IAutenticacaoPresenter
    {
        AutenticacaoUseCaseDto? ParaUseCaseDto(AutenticacaoRequest request);
        AutenticacaoResponse? ParaResponse(AutenticacaoDto autenticacaoUseCaseDto);
    }
}
