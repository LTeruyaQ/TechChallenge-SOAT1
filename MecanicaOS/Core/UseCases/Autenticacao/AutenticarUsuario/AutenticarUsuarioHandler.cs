using Core.DTOs.UseCases.Autenticacao;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Autenticacao;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Autenticacao.AutenticarUsuario
{
    public class AutenticarUsuarioHandler : UseCasesHandlerAbstrato<AutenticarUsuarioHandler>, IAutenticarUsuarioHandler
    {
        private readonly IUsuarioUseCases _usuarioUseCases;
        private readonly IClienteUseCases _clienteUseCases;
        private readonly ISegurancaGateway _segurancaGateway;

        public AutenticarUsuarioHandler(
            IUsuarioUseCases usuarioUseCases,
            ISegurancaGateway segurancaGateway,
            ILogGateway<AutenticarUsuarioHandler> logServicoGateway,
            IClienteUseCases clienteUseCases,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _usuarioUseCases = usuarioUseCases ?? throw new ArgumentNullException(nameof(usuarioUseCases));
            _segurancaGateway = segurancaGateway ?? throw new ArgumentNullException(nameof(segurancaGateway));
            _clienteUseCases = clienteUseCases ?? throw new ArgumentNullException(nameof(clienteUseCases));
        }

        public async Task<AutenticacaoDto> Handle(AutenticacaoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, request);

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                    throw new DadosInvalidosException("Email e senha são obrigatórios");

                var usuario = await _usuarioUseCases.ObterPorEmailUseCaseAsync(request.Email);

                if (usuario is null)
                    throw new DadosInvalidosException("Usuário ou senha inválidos");

                if (!_segurancaGateway.VerificarSenha(request.Senha, usuario.Senha))
                    throw new DadosInvalidosException("Usuário ou senha inválidos");

                var permissoes = ObterPermissoesDoUsuario(usuario);
                var token = _segurancaGateway.GerarToken(usuario.Id, usuario.Email, usuario.TipoUsuario.ToString(), null, permissoes);

                var autenticacaoDto = new AutenticacaoDto
                {
                    Token = token,
                    Usuario = usuario,
                    Permissoes = permissoes.ToList()
                };

                LogFim(metodo, autenticacaoDto);
                return autenticacaoDto;
            }
            catch (Exception ex)
            {
                LogErro(metodo, ex);
                throw;
            }
        }

        private IEnumerable<string> ObterPermissoesDoUsuario(Usuario usuario)
        {
            var permissoes = new List<string>();

            switch (usuario.TipoUsuario)
            {
                case TipoUsuario.Admin:
                    permissoes.Add("administrador");
                    break;

                case TipoUsuario.Cliente:
                    permissoes.Add("cliente");
                    break;
            }

            return permissoes;
        }
    }
}
