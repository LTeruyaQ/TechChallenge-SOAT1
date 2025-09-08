using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Usuario;
using Adapters.Presenters.Interfaces;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class UsuarioController
    {
        private readonly IUsuarioUseCases _usuarioUseCases;
        private readonly IUsuarioPresenter _usuarioPresenter;

        public UsuarioController(IUsuarioUseCases usuarioUseCases, IUsuarioPresenter usuarioPresenter)
        {
            _usuarioUseCases = usuarioUseCases;
            _usuarioPresenter = usuarioPresenter;
        }

        public async Task<IEnumerable<UsuarioResponse>> ObterTodos()
        {
            return _usuarioPresenter.ParaResponse(await _usuarioUseCases.ObterTodosUseCaseAsync());
        }

        public async Task<UsuarioResponse> ObterPorId(Guid id)
        {
            return _usuarioPresenter.ParaResponse(await _usuarioUseCases.ObterPorIdUseCaseAsync(id));
        }

        public async Task<UsuarioResponse> ObterPorEmail(string email)
        {
            return _usuarioPresenter.ParaResponse(await _usuarioUseCases.ObterPorEmailUseCaseAsync(email));
        }

        public async Task<UsuarioResponse> Cadastrar(CadastrarUsuarioRequest request)
        {
            return _usuarioPresenter.ParaResponse(
                await _usuarioUseCases.CadastrarUseCaseAsync(
                    _usuarioPresenter.ParaUseCaseDto(request)));
        }

        public async Task<UsuarioResponse> Atualizar(Guid id, AtualizarUsuarioRequest request)
        {
            return _usuarioPresenter.ParaResponse(
                await _usuarioUseCases.AtualizarUseCaseAsync(id,
                    _usuarioPresenter.ParaUseCaseDto(request)));
        }

        public async Task<bool> Deletar(Guid id)
        {
            return await _usuarioUseCases.DeletarUseCaseAsync(id);
        }
    }
}
