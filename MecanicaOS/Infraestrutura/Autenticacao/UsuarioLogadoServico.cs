using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MecanicaOS.Dominio.Interfaces.Servicos;
using Dominio.Interfaces.Repositorios;
using Dominio.Entidades;
using Dominio.Enumeradores;

namespace MecanicaOS.Infraestrutura.Autenticacao;

public class UsuarioLogadoServico : IUsuarioLogadoServico
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepositorio<Usuario> _usuarioRepositorio;

    public UsuarioLogadoServico(
        IHttpContextAccessor httpContextAccessor,
        IRepositorio<Usuario> usuarioRepositorio)
    {
        _httpContextAccessor = httpContextAccessor;
        _usuarioRepositorio = usuarioRepositorio;
    }

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;
    private ClaimsPrincipal? Usuario => HttpContext?.User;

    public Guid? UsuarioId
    {
        get
        {
            if (Usuario?.Identity?.IsAuthenticated != true)
                return null;

            var userIdClaim = Usuario.FindFirst(ClaimTypes.NameIdentifier) ?? 
                            Usuario.FindFirst("sub") ?? 
                            Usuario.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            return userId;
        }
    }

    public string? Email => Usuario?.FindFirst(ClaimTypes.Email)?.Value;
    public string? Nome => Usuario?.FindFirst(ClaimTypes.Name)?.Value;

    public TipoUsuario? TipoUsuario
    {
        get
        {
            if (Usuario?.Identity?.IsAuthenticated != true)
                return null;

            var tipoUsuarioClaim = Usuario.FindFirst("tipo_usuario");
            if (tipoUsuarioClaim == null || !Enum.TryParse<TipoUsuario>(tipoUsuarioClaim.Value, out var tipoUsuario))
                return null;

            return tipoUsuario;
        }
    }

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated ?? false;

    public bool EstaNaRole(string role)
    {
        return Usuario?.IsInRole(role) ?? false;
    }

    public bool PossuiPermissao(string permissao)
    {
        return Usuario?.HasClaim("permissao", permissao) ?? false;
    }

    public IEnumerable<Claim> ObterTodasClaims()
    {
        return Usuario?.Claims ?? Enumerable.Empty<Claim>();
    }

    public Usuario? ObterUsuarioLogado()
    {
        if (!EstaAutenticado || !UsuarioId.HasValue)
            return null;

        return _usuarioRepositorio.ObterPorIdAsync(UsuarioId.Value).Result;
    }
}
