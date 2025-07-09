using System;
using System.Security.Claims;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Http;

namespace Aplicacao.Servicos;

public class UsuarioLogado : IUsuarioLogado
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioLogado(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid? ObterId()
    {
        var id = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return !string.IsNullOrEmpty(id) && Guid.TryParse(id, out var guid) ? guid : null;
    }

    public string ObterLogin() => 
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public string ObterEmail() => 
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public string ObterNome() => 
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;

    public string ObterTipoUsuario() => 
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    public bool EstaAutenticado() => 
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
}
