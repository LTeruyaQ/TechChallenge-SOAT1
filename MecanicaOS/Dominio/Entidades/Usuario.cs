using Dominio.Entidades.Abstratos;
using Dominio.Enumeradores;

namespace Dominio.Entidades;

public class Usuario : Entidade
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public DateTime? DataUltimoAcesso { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public bool RecebeAlertaEstoque { get; set; }
}