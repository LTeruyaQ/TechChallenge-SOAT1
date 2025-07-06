using System.ComponentModel.DataAnnotations;

namespace Dominio.DTOs.Estoque;

public class EstoqueRegistrarDto
{
    [Required]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public required string Insumo { get; set; }

    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade disponível deve ser maior ou igual a zero.")]
    public int QuantidadeDisponivel { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser maior que zero.")]
    public int QuantidadeMinima { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser maior que zero.")]
    public double Preco { get; set; }
}
