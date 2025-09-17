using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Usuario;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.Usuario;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class UsuarioController : IUsuarioController
    {
        private readonly IUsuarioUseCases _usuarioUseCases;
        private readonly IUsuarioPresenter _usuarioPresenter;

        public UsuarioController(IUsuarioUseCases usuarioUseCases, IUsuarioPresenter usuarioPresenter)
        {
            _usuarioUseCases = usuarioUseCases;
            _usuarioPresenter = usuarioPresenter;
        }

        public async Task<IEnumerable<UsuarioResponse>> ObterTodosAsync()
        {
            return _usuarioPresenter.ParaResponse(await _usuarioUseCases.ObterTodosUseCaseAsync());
        }

        public async Task<UsuarioResponse> ObterPorIdAsync(Guid id)
        {
            return _usuarioPresenter.ParaResponse(await _usuarioUseCases.ObterPorIdUseCaseAsync(id));
        }

        public async Task<UsuarioResponse> ObterPorEmailAsync(string email)
        {
            return _usuarioPresenter.ParaResponse(await _usuarioUseCases.ObterPorEmailUseCaseAsync(email));
        }

        public async Task<UsuarioResponse> CadastrarAsync(CadastrarUsuarioRequest request)
        {
            var useCaseDto = MapearParaCadastrarUsuarioUseCaseDto(request);
            var resultado = await _usuarioUseCases.CadastrarUseCaseAsync(useCaseDto);
            return _usuarioPresenter.ParaResponse(resultado);
        }

        internal CadastrarUsuarioUseCaseDto MapearParaCadastrarUsuarioUseCaseDto(CadastrarUsuarioRequest request)
        {
            if (request is null)
                return null;

            return new CadastrarUsuarioUseCaseDto
            {
                Email = request.Email,
                Senha = request.Senha,
                TipoUsuario = request.TipoUsuario,
                RecebeAlertaEstoque = request.RecebeAlertaEstoque,
                Documento = request.Documento
            };
        }

        public async Task<UsuarioResponse> AtualizarAsync(Guid id, AtualizarUsuarioRequest request)
        {
            var useCaseDto = MapearParaAtualizarUsuarioUseCaseDto(request);
            var resultado = await _usuarioUseCases.AtualizarUseCaseAsync(id, useCaseDto);
            return _usuarioPresenter.ParaResponse(resultado);
        }

        internal AtualizarUsuarioUseCaseDto MapearParaAtualizarUsuarioUseCaseDto(AtualizarUsuarioRequest request)
        {
            if (request is null)
                return null;

            return new AtualizarUsuarioUseCaseDto
            {
                Email = request.Email,
                Senha = request.Senha,
                DataUltimoAcesso = request.DataUltimoAcesso,
                TipoUsuario = request.TipoUsuario,
                RecebeAlertaEstoque = request.RecebeAlertaEstoque
            };
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            return await _usuarioUseCases.DeletarUseCaseAsync(id);
        }
    }
}
