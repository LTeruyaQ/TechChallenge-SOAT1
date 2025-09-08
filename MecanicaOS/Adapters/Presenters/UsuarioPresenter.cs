using Adapters.Presenters.Interfaces;
using Aplicacao.DTOs.Requests.Usuario;
using Core.DTOs.Usuario;

namespace Adapters.Presenters
{
    public class UsuarioPresenter : IUsuarioPresenter
    {
        public CadastrarUsuarioUseCaseDto ParaUseCaseDto(CadastrarUsuarioRequest request)
        {
            if (request == null)
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

        public AtualizarUsuarioUseCaseDto ParaUseCaseDto(AtualizarUsuarioRequest request)
        {
            if (request == null)
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
    }
}
