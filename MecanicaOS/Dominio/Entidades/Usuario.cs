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

    public Guid? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public Usuario() { }

    public void Atualizar(string? email, string? senha, DateTime? dataUltimoAcesso, TipoUsuario? tipoUsuario, bool? recebeAlertaEstoque, bool? ativo = null)
    {
        if (!string.IsNullOrEmpty(email)) Email = email;
        if (!string.IsNullOrEmpty(senha)) Senha = senha;
        if (dataUltimoAcesso != null) DataUltimoAcesso = dataUltimoAcesso;
        if (tipoUsuario != null) TipoUsuario = tipoUsuario.Value;
        if (recebeAlertaEstoque != null) RecebeAlertaEstoque = recebeAlertaEstoque.Value;
        if (ativo != null) Ativo = ativo.Value;
    }

    public void AtualizarUltimoAcesso()
    {
        DataUltimoAcesso = DateTime.UtcNow;
    }
}