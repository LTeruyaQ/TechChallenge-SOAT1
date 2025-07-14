using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Usuario;
using Dominio.Entidades;

namespace Aplicacao.Interfaces.Servicos;

public interface IUsuarioServico
{
    Task<UsuarioResponse?> ObterPorIdAsync(Guid id);
    Task<UsuarioResponse> CadastrarAsync(CadastrarUsuarioRequest request);
    Task<UsuarioResponse> AtualizarAsync(Guid id, AtualizarUsuarioRequest request);
    Task<bool> DeletarAsync(Guid id);
    Task<IEnumerable<UsuarioResponse>> ObterTodosAsync();
    Task<Usuario?> ObterPorEmailAsync(string email);
}
