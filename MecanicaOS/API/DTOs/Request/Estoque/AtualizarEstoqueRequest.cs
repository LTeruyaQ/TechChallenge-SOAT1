using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request.Estoque
{
    public class AtualizarEstoqueRequest
    {
        [MaxLength(100)]
        public string? Insumo { get; set; } = default!;

        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Range(1, int.MaxValue)]
        public decimal? Preco { get; set; }

        [Range(1, int.MaxValue)]
        public int? QuantidadeMinima { get; set; }

        public int? QuantidadeDisponivel { get; set; }
    }
}