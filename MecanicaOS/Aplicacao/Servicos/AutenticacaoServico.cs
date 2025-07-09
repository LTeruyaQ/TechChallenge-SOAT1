using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Responses.Autenticacao;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aplicacao.Servicos;

public class AutenticacaoServico : ServicoAbstrato<AutenticacaoServico, Usuario>, IAutenticacaoServico
{
    private readonly IConfiguration _configuration;
    private readonly IRepositorio<Cliente> _clienteRepositorio;

    public AutenticacaoServico(
        IRepositorio<Usuario> repositorio,
        IRepositorio<Cliente> clienteRepositorio,
        ILogServico<AutenticacaoServico> logServico,
        IUnidadeDeTrabalho uot,
        IMapper mapper,
        IConfiguration configuration,
        IUsuarioLogado usuarioLogado)
        : base(repositorio, logServico, uot, mapper, usuarioLogado)
    {
        _configuration = configuration;
        _clienteRepositorio = clienteRepositorio;
    }

    public async Task<LoginResponse> AutenticarAsync(LoginRequest request)
    {
        LogInicio(nameof(AutenticarAsync), request);

        try
        {
            var usuario = await ObterUsuarioPorLoginComDadosRelacionados(request.Login);
            ValidarSenha(usuario, request.Senha);

            var response = GerarTokenJwt(usuario);
            LogFim(nameof(AutenticarAsync), new { usuario.Id, usuario.Email });

            return response;
        }
        catch (AutenticacaoException ex)
        {
            LogErro(nameof(AutenticarAsync), ex);
            throw;
        }
        catch (Exception ex)
        {
            LogErro(nameof(AutenticarAsync), ex);
            throw new AutenticacaoException("Falha ao autenticar usuário");
        }
    }

    public async Task<LoginResponse> RegistrarAsync(RegistroRequest request)
    {
        LogInicio(nameof(RegistrarAsync), request);
        Cliente cliente = null;

        try
        {
            await ValidarDadosRegistro(request);
            cliente = await ObterOuCriarCliente(request);
            var usuario = await CriarUsuario(request, cliente.Id);

            await Commit();

            var response = GerarTokenJwt(usuario);

            LogFim(nameof(RegistrarAsync), new
            {
                usuario.Id,
                usuario.Email,
                ClienteExistente = cliente.DataCadastro < DateTime.UtcNow.AddMinutes(-1)
            });

            return response;
        }
        catch (AutenticacaoException ex)
        {
            LogErro(nameof(RegistrarAsync), ex);
            throw;
        }
        catch (DadosInvalidosException ex)
        {
            LogErro(nameof(RegistrarAsync), ex);
            throw;
        }
        catch (Exception ex)
        {
            LogErro(nameof(RegistrarAsync), ex);
            throw new AutenticacaoException("Falha ao registrar usuário");
        }
    }

    public async Task<bool> ValidarCredenciaisAsync(string login, string senha)
    {
        LogInicio(nameof(ValidarCredenciaisAsync), new { login });

        try
        {
            var usuario = await ObterUsuarioPorLogin(login);
            var credenciaisValidas = ValidarSenhaSemLancarExcecao(usuario, senha);

            LogFim(nameof(ValidarCredenciaisAsync), new { login, valido = credenciaisValidas });
            return credenciaisValidas;
        }
        catch (Exception ex)
        {
            LogErro(nameof(ValidarCredenciaisAsync), ex);
            throw;
        }
    }

    #region Métodos Privados
    private async Task<Usuario> ObterUsuarioPorLoginComDadosRelacionados(string login)
    {
        LogInicio(nameof(ObterUsuarioPorLoginComDadosRelacionados), new { login });

        try
        {
            var usuario = await _repositorio.ObterUmAsync(
                new UsuarioPorEmailEspecificacao(login))
                ?? throw new UsuarioNaoEncontradoException(login);

            LogFim(nameof(ObterUsuarioPorLoginComDadosRelacionados), new { usuario.Id });
            return usuario;
        }
        catch (Exception ex)
        {
            LogErro(nameof(ObterUsuarioPorLoginComDadosRelacionados), ex);
            throw;
        }
    }

    private async Task<Usuario> ObterUsuarioPorLogin(string login)
    {
        LogInicio(nameof(ObterUsuarioPorLogin), new { login });

        try
        {
            var usuario = await _repositorio.ObterUmAsync(
                new UsuarioPorEmailEspecificacao(login));

            LogFim(nameof(ObterUsuarioPorLogin), new { login, usuarioEncontrado = usuario != null });
            return usuario;
        }
        catch (Exception ex)
        {
            LogErro(nameof(ObterUsuarioPorLogin), ex);
            throw;
        }
    }

    private void ValidarSenha(Usuario usuario, string senha)
    {
        LogInicio(nameof(ValidarSenha), new { usuarioId = usuario?.Id });

        try
        {
            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.Senha!))
                throw new CredenciaisInvalidasException();

            LogFim(nameof(ValidarSenha), new { usuarioId = usuario.Id });
        }
        catch (Exception ex)
        {
            LogErro(nameof(ValidarSenha), ex);
            throw;
        }
    }

    private bool ValidarSenhaSemLancarExcecao(Usuario usuario, string senha)
    {
        LogInicio(nameof(ValidarSenhaSemLancarExcecao), new { usuarioId = usuario?.Id });

        try
        {
            var valido = usuario != null && BCrypt.Net.BCrypt.Verify(senha, usuario.Senha!);
            LogFim(nameof(ValidarSenhaSemLancarExcecao), new { usuarioId = usuario?.Id, valido });
            return valido;
        }
        catch (Exception ex)
        {
            LogErro(nameof(ValidarSenhaSemLancarExcecao), ex);
            return false;
        }
    }

    private async Task ValidarDadosRegistro(RegistroRequest request)
    {
        LogInicio(nameof(ValidarDadosRegistro), new { request.Email });

        try
        {
            if (await LoginJaExiste(request.Email))
                throw new UsuarioJaCadastradoException(request.Email);

            if (request.Senha != request.ConfirmacaoSenha)
                throw new DadosInvalidosException("A senha e confirmação não conferem");

            LogFim(nameof(ValidarDadosRegistro), new { request.Email });
        }
        catch (Exception ex) when (ex is not (AutenticacaoException or DadosInvalidosException))
        {
            LogErro(nameof(ValidarDadosRegistro), ex);
            throw;
        }
    }

    private async Task<bool> LoginJaExiste(string login)
    {
        LogInicio(nameof(LoginJaExiste), new { login });

        try
        {
            var existe = await _repositorio.ObterUmAsync(
                new UsuarioPorEmailEspecificacao(login)) != null;

            LogFim(nameof(LoginJaExiste), new { login, existe });
            return existe;
        }
        catch (Exception ex)
        {
            LogErro(nameof(LoginJaExiste), ex);
            throw;
        }
    }

    private async Task<Cliente> ObterOuCriarCliente(RegistroRequest request)
    {
        LogInicio(nameof(ObterOuCriarCliente), new { request.Documento });

        try
        {
            var clienteExistente = await ObterClientePorDocumento(request.Documento);
            var cliente = clienteExistente != null
                ? await AtualizarClienteExistente(clienteExistente, request)
                : await CriarNovoCliente(request);

            LogFim(nameof(ObterOuCriarCliente), new { cliente.Id, cliente.Documento });
            return cliente;
        }
        catch (Exception ex)
        {
            LogErro(nameof(ObterOuCriarCliente), ex);
            throw;
        }
    }

    private async Task<Cliente> ObterClientePorDocumento(string documento)
    {
        LogInicio(nameof(ObterClientePorDocumento), new { documento });

        try
        {
            var cliente = await _clienteRepositorio.ObterUmAsync(
                new ClientePorDocumentoEspecificacao(documento)
                    .Incluir(c => c.Usuario)
                    .Incluir(c => c.Contato));

            LogFim(nameof(ObterClientePorDocumento), new { documento, encontrado = cliente != null });
            return cliente;
        }
        catch (Exception ex)
        {
            LogErro(nameof(ObterClientePorDocumento), ex);
            throw;
        }
    }

    private async Task<Cliente> AtualizarClienteExistente(Cliente cliente, RegistroRequest request)
    {
        LogInicio(nameof(AtualizarClienteExistente), new { cliente.Id, cliente.Documento });

        try
        {
            if (cliente.Usuario != null)
            {
                throw new ClienteJaPossuiCadastroException(request.Documento);
            }

            cliente.Nome = request.Nome;
            cliente.Contato ??= new Contato();
            cliente.Contato.Email = request.Email;
            cliente.Contato.Telefone = request.Telefone;

            await _clienteRepositorio.EditarAsync(cliente);

            LogFim(nameof(AtualizarClienteExistente), new { cliente.Id, cliente.Documento });
            return cliente;
        }
        catch (Exception ex)
        {
            LogErro(nameof(AtualizarClienteExistente), ex);
            throw;
        }
    }

    private async Task<Cliente> CriarNovoCliente(RegistroRequest request)
    {
        LogInicio(nameof(CriarNovoCliente), new { request.Documento });

        try
        {
            var cliente = new Cliente
            {
                Nome = request.Nome,
                Documento = request.Documento,
                TipoCliente = TipoUsuarioEnum.Cliente.ToString(),
                Contato = new Contato
                {
                    Email = request.Email,
                    Telefone = request.Telefone
                }
            };

            await _clienteRepositorio.CadastrarAsync(cliente);

            LogFim(nameof(CriarNovoCliente), new { cliente.Id, cliente.Documento });
            return cliente;
        }
        catch (Exception ex)
        {
            LogErro(nameof(CriarNovoCliente), ex);
            throw;
        }
    }

    private async Task<Usuario> CriarUsuario(RegistroRequest request, Guid clienteId)
    {
        LogInicio(nameof(CriarUsuario), new { request.Email, clienteId });

        try
        {
            var usuario = new Usuario
            {
                Email = request.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                TipoUsuario = TipoUsuarioEnum.Cliente
            };

            await _repositorio.CadastrarAsync(usuario);

            LogFim(nameof(CriarUsuario), new { usuario.Id, usuario.Email });
            return usuario;
        }
        catch (Exception ex)
        {
            LogErro(nameof(CriarUsuario), ex);
            throw;
        }
    }

    private LoginResponse GerarTokenJwt(Usuario usuario)
    {
        LogInicio(nameof(GerarTokenJwt), new { usuario.Id, usuario.Email });

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(ObterClaims(usuario)),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var response = new LoginResponse
            {
                Token = tokenString,
                DataExpiracao = token.ValidTo,
                TipoUsuario = usuario.TipoUsuario.ToString()
            };

            LogFim(nameof(GerarTokenJwt), new { usuario.Id, ExpiraEm = token.ValidTo });
            return response;
        }
        catch (Exception ex)
        {
            LogErro(nameof(GerarTokenJwt), ex);
            throw;
        }
    }

    private IEnumerable<Claim> ObterClaims(Usuario usuario)
    {
        LogInicio(nameof(ObterClaims), new { usuario.Id });

        try
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, usuario.Email!),
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Role, usuario.TipoUsuario.ToString())
            };

            LogFim(nameof(ObterClaims), new { usuario.Id, QuantidadeClaims = claims.Count });
            return claims;
        }
        catch (Exception ex)
        {
            LogErro(nameof(ObterClaims), ex);
            throw;
        }
    }

    #endregion
}