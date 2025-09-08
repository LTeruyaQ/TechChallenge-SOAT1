using Core.DTOs.Autenticacao;
using Core.DTOs.Usuario;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;

namespace Core.UseCases
{
    public class AutenticacaoUseCases : IAutenticacaoUseCases
    {
        private readonly IUsuarioUseCases _usuarioUseCases;
        private readonly IClienteUseCases _clienteUseCases;
        private readonly IServicoSenha _servicoSenha;
        private readonly IServicoJwt _servicoJwt;
        private readonly ILogServico<AutenticacaoUseCases> _log;

        public AutenticacaoUseCases(
            IUsuarioUseCases usuarioUseCases,
            IServicoSenha servicoSenha,
            IServicoJwt servicoJwt,
            ILogServico<AutenticacaoUseCases> log,
            IClienteUseCases clienteUseCases)
        {
            _usuarioUseCases = usuarioUseCases ?? throw new ArgumentNullException(nameof(usuarioUseCases));
            _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
            _servicoJwt = servicoJwt ?? throw new ArgumentNullException(nameof(servicoJwt));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _usuarioUseCases = usuarioUseCases ?? throw new ArgumentNullException(nameof(clienteUseCases));
        }

        public async Task<AutenticacaoDto> AutenticarUseCaseAsync(AutenticacaoUseCaseDto request)
        {
            const string metodo = nameof(AutenticarUseCaseAsync);
            _log.LogInicio(metodo, new { request.Email });

            try
            {
                var usuario = await _usuarioUseCases.ObterPorEmailUseCaseAsync(request.Email) ??
                    throw new CredenciaisInvalidasException("Credenciais inválidas");

                if (!usuario.Ativo)
                    throw new UsuarioInativoException("Usuário inativo. Entre em contato com o administrador.");

                if (!_servicoSenha.VerificarSenha(request.Senha, usuario.Senha))
                    throw new CredenciaisInvalidasException("Credenciais inválidas");


                var permissoes = ObterPermissoesDoUsuario(usuario);

                var token = _servicoJwt.GerarToken(
                    usuarioId: usuario.Id,
                    email: usuario.Email,
                    tipoUsuario: usuario.TipoUsuario.ToString(),
                    nome: await ObterNomeUsuario(usuario),
                    permissoes: permissoes);

                await _usuarioUseCases.AtualizarUseCaseAsync(usuario.Id, new AtualizarUsuarioUseCaseDto
                {
                    DataUltimoAcesso = DateTime.UtcNow
                });

                var result = new AutenticacaoDto { Token = token };

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

            var cliente = await _clienteUseCases.ObterPorIdUseCaseAsync(usuario.ClienteId.Value);

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
