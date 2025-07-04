using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Veiculo : Entidade
{
    public string Placa { get; set; } = default!;
    public string Marca { get; set; } = default!;
    public string Modelo { get; set; } = default!;
    public string Cor { get; set; } = default!;
    public string Ano { get; set; } = default!;
    public string? Anotacoes { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }

    // Relacionamento com Cliente (opcional ou obrigatório?)
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = default!;
}
