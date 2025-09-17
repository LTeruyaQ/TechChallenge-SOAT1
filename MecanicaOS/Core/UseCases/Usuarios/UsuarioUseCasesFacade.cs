using Core.DTOs.UseCases.Usuario;
using Core.Entidades;
using Core.Interfaces.UseCases;
using Core.UseCases.Usuarios.AtualizarUsuario;
using Core.UseCases.Usuarios.CadastrarUsuario;
using Core.UseCases.Usuarios.DeletarUsuario;
using Core.UseCases.Usuarios.ObterTodosUsuarios;
using Core.UseCases.Usuarios.ObterUsuario;
using Core.UseCases.Usuarios.ObterUsuarioPorEmail;

namespace Core.UseCases.Usuarios
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IUsuarioUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class UsuarioUseCasesFacade : IUsuarioUseCases
    {
        private readonly CadastrarUsuarioHandler _cadastrarUsuarioHandler;
        private readonly AtualizarUsuarioHandler _atualizarUsuarioHandler;
        private readonly ObterUsuarioHandler _obterUsuarioHandler;
        private readonly ObterTodosUsuariosHandler _obterTodosUsuariosHandler;
        private readonly DeletarUsuarioHandler _deletarUsuarioHandler;
        private readonly ObterUsuarioPorEmailHandler _obterUsuarioPorEmailHandler;

        public UsuarioUseCasesFacade(
            CadastrarUsuarioHandler cadastrarUsuarioHandler,
            AtualizarUsuarioHandler atualizarUsuarioHandler,
            ObterUsuarioHandler obterUsuarioHandler,
            ObterTodosUsuariosHandler obterTodosUsuariosHandler,
            DeletarUsuarioHandler deletarUsuarioHandler,
            ObterUsuarioPorEmailHandler obterUsuarioPorEmailHandler)
        {
            _cadastrarUsuarioHandler = cadastrarUsuarioHandler ?? throw new ArgumentNullException(nameof(cadastrarUsuarioHandler));
            _atualizarUsuarioHandler = atualizarUsuarioHandler ?? throw new ArgumentNullException(nameof(atualizarUsuarioHandler));
            _obterUsuarioHandler = obterUsuarioHandler ?? throw new ArgumentNullException(nameof(obterUsuarioHandler));
            _obterTodosUsuariosHandler = obterTodosUsuariosHandler ?? throw new ArgumentNullException(nameof(obterTodosUsuariosHandler));
            _deletarUsuarioHandler = deletarUsuarioHandler ?? throw new ArgumentNullException(nameof(deletarUsuarioHandler));
            _obterUsuarioPorEmailHandler = obterUsuarioPorEmailHandler ?? throw new ArgumentNullException(nameof(obterUsuarioPorEmailHandler));
        }

        public async Task<Usuario> AtualizarUseCaseAsync(Guid id, AtualizarUsuarioUseCaseDto request)
        {
            var response = await _atualizarUsuarioHandler.Handle(id, request);
            return response.Usuario;
        }

        public async Task<Usuario> CadastrarUseCaseAsync(CadastrarUsuarioUseCaseDto request)
        {
            var response = await _cadastrarUsuarioHandler.Handle(request);
            return response.Usuario;
        }

        public async Task<bool> DeletarUseCaseAsync(Guid id)
        {
            var response = await _deletarUsuarioHandler.Handle(id);
            return response.Sucesso;
        }

        public async Task<Usuario?> ObterPorEmailUseCaseAsync(string email)
        {
            var response = await _obterUsuarioPorEmailHandler.Handle(email);
            return response.Usuario;
        }

        public async Task<Usuario?> ObterPorIdUseCaseAsync(Guid id)
        {
            var response = await _obterUsuarioHandler.Handle(id);
            return response.Usuario;
        }

        public async Task<IEnumerable<Usuario>> ObterTodosUseCaseAsync()
        {
            var response = await _obterTodosUsuariosHandler.Handle();
            return response.Usuarios;
        }
    }
}
