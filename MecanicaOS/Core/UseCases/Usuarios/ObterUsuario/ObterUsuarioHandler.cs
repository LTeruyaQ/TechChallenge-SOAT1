using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Usuarios.ObterUsuario
{
    public class ObterUsuarioHandler : UseCasesAbstrato<ObterUsuarioHandler, Usuario>
    {
        private readonly IUsuarioGateway _usuarioGateway;

        public ObterUsuarioHandler(
            IUsuarioGateway usuarioGateway,
            ILogServico<ObterUsuarioHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _usuarioGateway = usuarioGateway ?? throw new ArgumentNullException(nameof(usuarioGateway));
        }

        public async Task<ObterUsuarioResponse> Handle(Guid id)
        {
            var metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var usuario = await _usuarioGateway.ObterPorIdAsync(id);
                
                if (usuario is not null)
                    IsNotGetSenha(usuario);

                LogFim(metodo, usuario);

                return new ObterUsuarioResponse { Usuario = usuario };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        private static void IsNotGetSenha(Usuario usuario)
        {
            usuario.Senha = string.Empty;
        }
    }
}
