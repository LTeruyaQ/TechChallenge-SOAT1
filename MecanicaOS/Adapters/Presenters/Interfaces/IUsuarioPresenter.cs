using Aplicacao.DTOs.Requests.Usuario;
using Core.DTOs.Usuario;

namespace Adapters.Presenters.Interfaces
{
    public interface IUsuarioPresenter
    {
        CadastrarUsuarioUseCaseDto ParaUseCaseDto(CadastrarUsuarioRequest request);
        AtualizarUsuarioUseCaseDto ParaUseCaseDto(AtualizarUsuarioRequest request);
    }
}
