using Core.Entidades;
using Core.Especificacoes.Usuario;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class UsuarioGateway : IUsuarioGateway
    {
        private readonly IRepositorio<Usuario> _repositorioUsuario;

        public UsuarioGateway(IRepositorio<Usuario> repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario;
        }

        public async Task<Usuario> CadastrarAsync(Usuario usuario)
        {
            return await _repositorioUsuario.CadastrarAsync(usuario);
        }

        public async Task DeletarAsync(Usuario usuario)
        {
            await _repositorioUsuario.DeletarAsync(usuario);
        }

        public async Task EditarAsync(Usuario usuario)
        {
            await _repositorioUsuario.EditarAsync(usuario);
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            var especificacao = new ObterUsuarioPorEmailEspecificacao(email);
            return await _repositorioUsuario.ObterUmSemRastreamentoAsync(especificacao);
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _repositorioUsuario.ObterPorIdAsync(id);
        }

        public async Task<IEnumerable<Usuario>> ObterTodosAsync()
        {
            return await _repositorioUsuario.ObterTodosAsync();
        }

        public async Task<IEnumerable<Usuario>> ObterUsuarioParaAlertaEstoqueAsync()
        {
            var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();
            return await _repositorioUsuario.ListarAsync(especificacao);
        }
    }
}
