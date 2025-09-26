using Core.DTOs.UseCases.Usuario;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Usuarios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Usuarios.CadastrarUsuario
{
    public class CadastrarUsuarioHandler : UseCasesHandlerAbstrato<CadastrarUsuarioHandler>, ICadastrarUsuarioHandler
    {
        private readonly IUsuarioGateway _usuarioGateway;
        private readonly IClienteUseCases _clienteUseCases;
        private readonly IServicoSenha _servicoSenha;

        public CadastrarUsuarioHandler(
            IUsuarioGateway usuarioGateway,
            IClienteUseCases clienteUseCases,
            ILogGateway<CadastrarUsuarioHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway,
            IServicoSenha servicoSenha)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _usuarioGateway = usuarioGateway ?? throw new ArgumentNullException(nameof(usuarioGateway));
            _clienteUseCases = clienteUseCases ?? throw new ArgumentNullException(nameof(clienteUseCases));
            _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
        }

        public async Task<CadastrarUsuarioResponse> Handle(CadastrarUsuarioUseCaseDto request)
        {
            var metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, request);

                await VerificarUsuarioCadastradoAsync(request.Email);

                Usuario usuario = new()
                {
                    Email = request.Email,
                    TipoUsuario = request.TipoUsuario,
                    RecebeAlertaEstoque = request.RecebeAlertaEstoque.HasValue ? request.RecebeAlertaEstoque.Value : false,
                };

                usuario.Senha = _servicoSenha.CriptografarSenha(request.Senha);

                if (request.TipoUsuario == TipoUsuario.Cliente)
                    usuario.ClienteId = await GetClienteIdUseCaseAsync(request.Documento);

                var entidade = await _usuarioGateway.CadastrarAsync(usuario);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar usuário");

                IsNotGetSenha(usuario);

                LogFim(metodo, usuario);

                return new CadastrarUsuarioResponse { Usuario = usuario };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        private async Task<Guid> GetClienteIdUseCaseAsync(string? documento)
        {
            string metodo = nameof(GetClienteIdUseCaseAsync);

            LogInicio(metodo, new { documento });

            try
            {
                _ = documento ?? throw new DadosInvalidosException("Usuários do tipo cliente devem informar o documento.");

                var cliente = await _clienteUseCases.ObterPorDocumentoUseCaseAsync(documento);

                LogFim(metodo);

                return cliente.Id;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        private async Task VerificarUsuarioCadastradoAsync(string email)
        {
            var metodo = nameof(VerificarUsuarioCadastradoAsync);

            try
            {
                LogInicio(metodo);

                var usuario = await _usuarioGateway.ObterPorEmailAsync(email);

                LogFim(metodo);

                if (usuario is not null)
                    throw new DadosJaCadastradosException("Usuário já cadastrado");
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
