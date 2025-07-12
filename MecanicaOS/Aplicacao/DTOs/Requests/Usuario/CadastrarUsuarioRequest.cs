using Dominio.Enumeradores;
using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Requests.Usuario;

public class CadastrarUsuarioRequest
{
    [Required]
    [MaxLength(240, ErrorMessage = "O campo deve ter no máximo 240 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public TipoUsuario TipoUsuario { get; set; }

    public bool? RecebeAlertaEstoque { get; set; }

    public string Documento { get; set; }
}
