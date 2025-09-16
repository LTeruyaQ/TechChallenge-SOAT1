using API.Filters;
using Core.Interfaces.Servicos;
using Infraestrutura.Dados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsuarioLogadoController : BaseApiController
{
    private readonly IUsuarioLogadoServico _usuarioLogadoServico;

    public UsuarioLogadoController(
        MecanicaContexto contexto,
        Mediator mediator,
        IServicoEmail servicoEmail,
        IIdCorrelacionalService idCorrelacionalService,
        HttpContextAccessor httpContext)
    {
        // Usando o CompositionRoot para obter o serviço de usuário logado
        var compositionRoot = new CompositionRoot(contexto, mediator, servicoEmail, idCorrelacionalService, httpContext);
        _usuarioLogadoServico = httpContext.HttpContext?.User.Identity?.IsAuthenticated == true
            ? compositionRoot.CreateUsuarioLogadoServico()
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
