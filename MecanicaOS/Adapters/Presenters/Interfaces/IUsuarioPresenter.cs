using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Usuario;
using Core.DTOs.UseCases.Usuario;
using Core.Entidades;

namespace Adapters.Presenters.Interfaces
{
    public interface IUsuarioPresenter
    {
        CadastrarUsuarioUseCaseDto? ParaUseCaseDto(CadastrarUsuarioRequest request);
        AtualizarUsuarioUseCaseDto? ParaUseCaseDto(AtualizarUsuarioRequest request);
        UsuarioResponse? ParaResponse(Usuario usuario);
        IEnumerable<UsuarioResponse?> ParaResponse(IEnumerable<Usuario> usuarios);
    }
}
