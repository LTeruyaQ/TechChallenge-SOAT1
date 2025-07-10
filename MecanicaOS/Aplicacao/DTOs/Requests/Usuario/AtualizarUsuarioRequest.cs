using Dominio.Enumeradores;
using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Requests.Usuario;

public class AtualizarUsuarioRequest
{
    [MaxLength(240, ErrorMessage = "O campo deve ter no máximo 240 caracteres.")]
    public string? Email { get; set; }

    public string? Senha { get; set; }

    public DateTime? DataUltimoAcesso { get; set; }

    [MaxLength(50, ErrorMessage = "O campo deve ter no máximo 50 caracteres.")]
    public TipoUsuario? TipoUsuario { get; set; }

    public bool? RecebeAlertaEstoque { get; set; }
}