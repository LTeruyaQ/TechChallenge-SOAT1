using API.Filters;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsuarioLogadoController : BaseApiController
{
    private readonly IUsuarioLogadoServico _usuarioLogadoServico;

    public UsuarioLogadoController(
        HttpContextAccessor httpContext,
        ICompositionRoot compositionRoot)
    {
        _usuarioLogadoServico = httpContext.HttpContext?.User.Identity?.IsAuthenticated == true
            ? compositionRoot.CriarUsuarioLogadoServico()
            : null;
    }

    [HttpGet("dados")]
    public IActionResult ObterDadosUsuarioLogado()
    {
        if (!_usuarioLogadoServico.EstaAutenticado)
            return Unauthorized("Usuário não autenticado");

        var usuario = _usuarioLogadoServico.ObterUsuarioLogado();

        return Ok(new
        {
            usuario.Id,
            usuario.Email,
            usuario.TipoUsuario,
            _usuarioLogadoServico.EstaAutenticado,
            Claims = _usuarioLogadoServico.ObterTodasClaims()
                .ToDictionary(c => c.Type, c => c.Value)
        });
    }

    [HttpGet("permissoes")]
    public IActionResult VerificarPermissoes()
    {
        if (!_usuarioLogadoServico.EstaAutenticado)
            return Unauthorized("Usuário não autenticado");

        return Ok(new
        {
            Cliente = _usuarioLogadoServico.PossuiPermissao("cliente"),
            Administrador = _usuarioLogadoServico.PossuiPermissao("administrador")
        });
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult ApenasAdmin()
    {
        return Ok(new { Mensagem = "Acesso permitido apenas para administradores" });
    }

    [HttpGet("gerenciar-estoque")]
    [PermissaoNecessaria("cliente")]
    public IActionResult ApenasComPermissaoGerenciarEstoque()
    {
        return Ok(new { Mensagem = "Acesso permitido apenas para usuários com permissão de gerenciar estoque" });
    }
}
