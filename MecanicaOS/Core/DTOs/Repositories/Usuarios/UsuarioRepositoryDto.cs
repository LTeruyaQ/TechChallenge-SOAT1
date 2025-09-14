using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Cliente;
using Core.Enumeradores;

namespace Core.DTOs.Repositories.Usuarios;

public class UsuarioRepositoryDto : RepositoryDto
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public DateTime? DataUltimoAcesso { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public bool RecebeAlertaEstoque { get; set; }

    public Guid? ClienteId { get; set; }
    public ClienteRepositoryDTO? Cliente { get; set; }
}