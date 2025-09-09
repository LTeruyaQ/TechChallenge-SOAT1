using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Usuario;

namespace Adapters.Controllers
{
    public interface IUsuarioController
    {
        Task<UsuarioResponse> AtualizarAsync(Guid id, AtualizarUsuarioRequest request);
        Task<UsuarioResponse> CadastrarAsync(CadastrarUsuarioRequest request);
        Task<bool> DeletarAsync(Guid id);
        Task<UsuarioResponse> ObterPorEmailAsync(string email);
        Task<UsuarioResponse> ObterPorIdAsync(Guid id);
        Task<IEnumerable<UsuarioResponse>> ObterTodosAsync();
    }
}