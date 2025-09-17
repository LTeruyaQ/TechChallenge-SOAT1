using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Usuarios.DeletarUsuario
{
    public class DeletarUsuarioHandler : UseCasesAbstrato<DeletarUsuarioHandler, Usuario>
    {
        private readonly IUsuarioGateway _usuarioGateway;

        public DeletarUsuarioHandler(
            IUsuarioGateway usuarioGateway,
            ILogServico<DeletarUsuarioHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _usuarioGateway = usuarioGateway ?? throw new ArgumentNullException(nameof(usuarioGateway));
        }

        public async Task<DeletarUsuarioResponse> Handle(Guid id)
        {
            var metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var usuario = await _usuarioGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Usuário não encontrado");

                await _usuarioGateway.DeletarAsync(usuario);
                var sucesso = await Commit();

                if (!sucesso)
                    throw new PersistirDadosException("Erro ao deletar usuário");

                LogFim(metodo, sucesso);

                return new DeletarUsuarioResponse { Sucesso = sucesso };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
