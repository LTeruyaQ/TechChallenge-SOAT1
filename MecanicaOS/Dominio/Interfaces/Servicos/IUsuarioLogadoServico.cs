using Dominio.Entidades;
using Dominio.Enumeradores;
using System.Security.Claims;

namespace Dominio.Interfaces.Servicos;

public interface IUsuarioLogadoServico
{
    Guid? UsuarioId { get; }
    string? Email { get; }
    string Nome { get; }
    TipoUsuario? TipoUsuario { get; }
    bool EstaAutenticado { get; }
    bool EstaNaRole(string role);
    bool PossuiPermissao(string permissao);
    IEnumerable<Claim> ObterTodasClaims();
    Usuario? ObterUsuarioLogado();
}
