using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Autenticacao;
using Aplicacao.Interfaces.Servicos;
using Dominio.Exceptions;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos
{
    public class AutenticacaoServico : IAutenticacaoServico
    {
        private readonly IUsuarioServico _usuarioServico;
        private readonly IServicoSenha _servicoSenha;
        private readonly IServicoJwt _servicoJwt;
        private readonly ILogServico<AutenticacaoServico> _log;

        public AutenticacaoServico(
            IUsuarioServico usuarioServico,
            IServicoSenha servicoSenha,
            IServicoJwt servicoJwt,
            ILogServico<AutenticacaoServico> log)
        {
            _usuarioServico = usuarioServico ?? throw new ArgumentNullException(nameof(usuarioServico));
            _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
            _servicoJwt = servicoJwt ?? throw new ArgumentNullException(nameof(servicoJwt));
            _log = log ?? throw new ArgumentNullException(nameof(log));
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

                await _usuarioServico.AtualizarAsync(usuario.Id, new AtualizarUsuarioRequest
                {
                    DataUltimoAcesso = DateTime.UtcNow
                });

                var token = _servicoJwt.GerarToken(
                    usuario.Id,
                    usuario.Email,
                    usuario.TipoUsuario.ToString());

                _log.LogFim(metodo, "Autenticação realizada com sucesso");
                return new AutenticacaoResponse { Token = token };
            }
            catch (Exception ex)
            {
                _log.LogErro(metodo, ex);
                throw;
            }
        }
    }
}
