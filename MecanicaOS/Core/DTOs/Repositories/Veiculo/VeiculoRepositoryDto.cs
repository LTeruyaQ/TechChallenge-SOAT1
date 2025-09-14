using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Cliente;

namespace Core.DTOs.Repositories.Veiculo;

public class VeiculoRepositoryDto : RepositoryDto
{
    public string Placa { get; set; } = default!;
    public string Marca { get; set; } = default!;
    public string Modelo { get; set; } = default!;
    public string Cor { get; set; } = default!;
    public string Ano { get; set; } = default!;
    public string? Anotacoes { get; set; }

    public Guid? ClienteId { get; set; }
    public ClienteRepositoryDTO Cliente { get; set; } = default!;
}
