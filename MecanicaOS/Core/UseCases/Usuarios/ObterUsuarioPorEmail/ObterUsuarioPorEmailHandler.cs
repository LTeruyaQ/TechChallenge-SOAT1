using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Usuarios.ObterUsuarioPorEmail
{
    public class ObterUsuarioPorEmailHandler : UseCasesAbstrato<ObterUsuarioPorEmailHandler, Usuario>
    {
        private readonly IUsuarioGateway _usuarioGateway;

        public ObterUsuarioPorEmailHandler(
            IUsuarioGateway usuarioGateway,
            ILogServico<ObterUsuarioPorEmailHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _usuarioGateway = usuarioGateway ?? throw new ArgumentNullException(nameof(usuarioGateway));
        }

        public async Task<ObterUsuarioPorEmailResponse> Handle(ObterUsuarioPorEmailUseCase query)
        {
            var metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, query.Email);

                var usuario = await _usuarioGateway.ObterPorEmailAsync(query.Email);
                
                if (usuario is not null)
                    IsNotGetSenha(usuario);

                LogFim(metodo, usuario);

                return new ObterUsuarioPorEmailResponse { Usuario = usuario };
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
