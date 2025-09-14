using Core.DTOs.Repositories.Autenticacao;

namespace Core.DTOs.Repositories.Cliente;

public class ContatoRepositoryDTO : RepositoryDto
{
    public Guid IdCliente { get; set; }
    public ClienteRepositoryDTO Cliente { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
}