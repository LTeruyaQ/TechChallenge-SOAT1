using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class PermissaoNecessariaAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _permissoes;

    public PermissaoNecessariaAttribute(params string[] permissoes)
    {
        _permissoes = permissoes ?? throw new ArgumentNullException(nameof(permissoes));
        
        if (permissoes.Length == 0)
        {
            throw new ArgumentException("Pelo menos uma permissão deve ser fornecida", nameof(permissoes));
        }
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var usuarioLogadoServico = context.HttpContext.RequestServices.GetService<IUsuarioLogadoServico>();

        if (usuarioLogadoServico == null || !usuarioLogadoServico.EstaAutenticado)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!_permissoes.Any(permissao => usuarioLogadoServico.PossuiPermissao(permissao)))
        {
            var permissoesFormatadas = string.Join(" ou ", _permissoes);
            context.Result = new ForbidResult($"Você precisa ter pelo menos uma das seguintes permissões: {permissoesFormatadas}");
            return;
        }
    }
}
