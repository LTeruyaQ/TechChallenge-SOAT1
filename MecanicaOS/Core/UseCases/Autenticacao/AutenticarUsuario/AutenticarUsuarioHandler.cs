using Core.DTOs.UseCases.Autenticacao;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Handlers.Autenticacao;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Autenticacao.AutenticarUsuario
{
    public class AutenticarUsuarioHandler : UseCasesHandlerAbstrato<AutenticarUsuarioHandler>, IAutenticarUsuarioHandler
    {
        private readonly IUsuarioUseCases _usuarioUseCases;
        private readonly IClienteUseCases _clienteUseCases;
        private readonly IServicoSenha _servicoSenha;
        private readonly IServicoJwt _servicoJwt;

        public AutenticarUsuarioHandler(
            IUsuarioUseCases usuarioUseCases,
            IServicoSenha servicoSenha,
            IServicoJwt servicoJwt,
            ILogServico<AutenticarUsuarioHandler> logServico,
            IClienteUseCases clienteUseCases,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _usuarioUseCases = usuarioUseCases ?? throw new ArgumentNullException(nameof(usuarioUseCases));
            _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
            _servicoJwt = servicoJwt ?? throw new ArgumentNullException(nameof(servicoJwt));
            _clienteUseCases = clienteUseCases ?? throw new ArgumentNullException(nameof(clienteUseCases));
        }

        public async Task<AutenticarUsuarioResponse> Handle(AutenticacaoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, request);

                var usuario = await _usuarioUseCases.ObterPorEmailUseCaseAsync(request.Email);

                if (usuario is null)
                    throw new DadosInvalidosException("Usuário ou senha inválidos");

                if (!_servicoSenha.VerificarSenha(request.Senha, usuario.Senha))
                    throw new DadosInvalidosException("Usuário ou senha inválidos");

                var permissoes = ObterPermissoesDoUsuario(usuario);
                var token = _servicoJwt.GerarToken(usuario.Id, usuario.Email, usuario.TipoUsuario.ToString(), null, permissoes);

                var autenticacaoDto = new AutenticacaoDto
                {
                    Token = token,
                    Usuario = usuario,
                    Permissoes = permissoes.ToList()
                };

                var response = new AutenticarUsuarioResponse { Autenticacao = autenticacaoDto };
                LogFim(metodo, response);
                return response;
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
