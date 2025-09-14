using Core.DTOs.Repositories.Autenticacao;

namespace Core.DTOs.Repositories.Cliente;

public class EnderecoRepositoryDto : RepositoryDto
{
    public string? Rua { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Numero { get; set; }
    public string? CEP { get; set; }
    public string? Complemento { get; set; }
    public Guid IdCliente { get; set; }
    public ClienteRepositoryDTO Cliente { get; set; } = default!;
}