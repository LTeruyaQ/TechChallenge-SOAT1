using System.ComponentModel.DataAnnotations;

namespace Aplicacao.UseCases.Estoque.CriarEstoque
{
    public class CriarEstoqueRequest
    {
        [Required]
        [MaxLength(100)]
        public string Insumo { get; set; } = default!;

        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public decimal Preco { get; set; }

        [Required]
        public int QuantidadeDisponivel { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int QuantidadeMinima { get; set; }
    }
}
