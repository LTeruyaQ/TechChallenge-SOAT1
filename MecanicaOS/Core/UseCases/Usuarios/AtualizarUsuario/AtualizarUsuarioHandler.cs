using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Usuarios.AtualizarUsuario
{
    public class AtualizarUsuarioHandler : UseCasesAbstrato<AtualizarUsuarioHandler, Usuario>
    {
        private readonly IUsuarioGateway _usuarioGateway;
        private readonly IServicoSenha _servicoSenha;

        public AtualizarUsuarioHandler(
            IUsuarioGateway usuarioGateway,
            IServicoSenha servicoSenha,
            ILogServico<AtualizarUsuarioHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _usuarioGateway = usuarioGateway ?? throw new ArgumentNullException(nameof(usuarioGateway));
            _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
        }

        public async Task<AtualizarUsuarioResponse> Handle(AtualizarUsuarioCommand command)
        {
            var metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { command.Id, command.Request });

                Usuario usuario = await _usuarioGateway.ObterPorIdAsync(command.Id) ?? throw new DadosNaoEncontradosException("Usuário não encontrado");

                string senhaCriptografada = !string.IsNullOrEmpty(command.Request.Senha)
                    ? _servicoSenha.CriptografarSenha(command.Request.Senha)
                    : usuario.Senha;

                usuario.Atualizar(
                    command.Request.Email,
                    senhaCriptografada,
                    command.Request.DataUltimoAcesso,
                    command.Request.TipoUsuario,
                    command.Request.RecebeAlertaEstoque);

                await _usuarioGateway.EditarAsync(usuario);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar usuario");
                
                IsNotGetSenha(usuario);

                LogFim(metodo, usuario);

                return new AtualizarUsuarioResponse { Usuario = usuario };
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
