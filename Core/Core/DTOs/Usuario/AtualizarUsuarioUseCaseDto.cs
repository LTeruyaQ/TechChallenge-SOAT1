using Core.Enumeradores;

namespace Core.DTOs.Usuario;

public class AtualizarUsuarioUseCaseDto
{
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public DateTime? DataUltimoAcesso { get; set; }
    public TipoUsuario? TipoUsuario { get; set; }
    public bool? RecebeAlertaEstoque { get; set; }
}