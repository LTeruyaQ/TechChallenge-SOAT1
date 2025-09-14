using Core.DTOs.Repositories.Autenticacao;

namespace Core.DTOs.Repositories.Estoque;

public class EstoqueRepositoryDto : RepositoryDto
{
    public string Insumo { get; set; } = default!;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public int QuantidadeMinima { get; set; }
}