using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Estoque : Entidade
{
    public string Insumo { get; set; } = default!;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public int QuantidadeMinima { get; set; }
}