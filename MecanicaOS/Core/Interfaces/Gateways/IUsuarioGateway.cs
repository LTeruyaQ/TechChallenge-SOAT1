using Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Gateways
{
    public interface IUsuarioGateway
    {
        Task<Usuario> CadastrarAsync(Usuario usuario);
        Task DeletarAsync(Usuario usuario);
        Task EditarAsync(Usuario usuario);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Usuario>> ObterTodosAsync();
        Task<IEnumerable<Usuario>> ObterUsuarioParaAlertaEstoqueAsync();
    }
}
