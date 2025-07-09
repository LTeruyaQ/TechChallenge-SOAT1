using Dominio.Entidades.Abstratos;
using Dominio.Enumeradores;

namespace Dominio.Entidades;

public class Usuario : Entidade
{
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public DateTime? DataUltimoAcesso { get; set; }
    public TipoUsuarioEnum? TipoUsuario { get; set; }
}