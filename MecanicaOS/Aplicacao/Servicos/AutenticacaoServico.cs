using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Autenticacao;
using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos
{
    public class AutenticacaoServico : IAutenticacaoServico
    {
        private readonly IUsuarioServico _usuarioServico;
        private readonly IClienteServico _clienteServico;
        private readonly IServicoSenha _servicoSenha;
        private readonly IServicoJwt _servicoJwt;
        private readonly ILogServico<AutenticacaoServico> _log;

        public AutenticacaoServico(
            IUsuarioServico usuarioServico,
            IServicoSenha servicoSenha,
            IServicoJwt servicoJwt,
            ILogServico<AutenticacaoServico> log,
            IClienteServico clienteServico)
        {
            _usuarioServico = usuarioServico ?? throw new ArgumentNullException(nameof(usuarioServico));
            _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
            _servicoJwt = servicoJwt ?? throw new ArgumentNullException(nameof(servicoJwt));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _clienteServico = clienteServico ?? throw new ArgumentNullException(nameof(clienteServico));
        }

        public async Task<AutenticacaoResponse> AutenticarAsync(AutenticacaoRequest request)
        {
            const string metodo = nameof(AutenticarAsync);
            _log.LogInicio(metodo, new { request.Email });

            try
            {
                var usuario = await _usuarioServico.ObterPorEmailAsync(request.Email) ??
                    throw new CredenciaisInvalidasException("E-mail ou senha inválidos");

                if (!usuario.Ativo)
                    throw new UsuarioInativoException("Usuário inativo. Entre em contato com o administrador.");

                if (!_servicoSenha.VerificarSenha(request.Senha, usuario.Senha))
                    throw new CredenciaisInvalidasException("E-mail ou senha inválidos");


                var permissoes = ObterPermissoesDoUsuario(usuario);

                var token = _servicoJwt.GerarToken(
                    usuarioId: usuario.Id,
                    email: usuario.Email,
                    tipoUsuario: usuario.TipoUsuario.ToString(),
                    nome: await ObterNomeUsuario(usuario),
                    permissoes: permissoes);

                await _usuarioServico.AtualizarAsync(usuario.Id, new AtualizarUsuarioRequest
                {
                    DataUltimoAcesso = DateTime.UtcNow
                });

                var result = new AutenticacaoResponse { Token = token };

                _log.LogFim(metodo, result);
                return result;
            }
            catch (Exception ex)
            {
                _log.LogErro(metodo, ex);
                throw;
            }
        }

        private async Task<string> ObterNomeUsuario(Usuario usuario)
        {
            var metodo = nameof(ObterNomeUsuario);
            _log.LogInicio(metodo, usuario);
            try
            {
                var nome = usuario.TipoUsuario == TipoUsuario.Cliente ?
                    await ObterNomeCliente(usuario) ?? usuario.Email :
                    usuario.Email;

                _log.LogFim(metodo, nome);
                return nome;
            }
            catch (Exception e)
            {
                _log.LogErro(metodo, e);
                throw;
            }
        }

        private async Task<string?> ObterNomeCliente(Usuario usuario)
        {
            if (!usuario.ClienteId.HasValue) throw new DadosInvalidosException("Erro ao detectar usuario, por favor associe um cliente a esse usuário");

            var cliente = await _clienteServico.ObterPorIdAsync(usuario.ClienteId.Value);

            return cliente.Nome;
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
