using Adapters.DTOs.Requests.Autenticacao;
using Core.DTOs.Autenticacao;

namespace Adapters.Presenters.Interfaces
{
    public interface IAutenticacaoPresenter
    {
        AutenticacaoUseCaseDto ParaUseCaseDto(AutenticacaoRequest request);
    }
}
