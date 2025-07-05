using System.ComponentModel.DataAnnotations;

namespace Dominio.DTOs.Estoque;

public class EstoqueAtualizarDto
{
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string? Insumo { get; set; }

    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade disponível deve ser maior ou igual a zero.")]
    public int? QuantidadeDisponivel { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser maior que zero.")]
    public int? QuantidadeMinima { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser maior que zero.")]
    public double? Preco { get; set; }
}
