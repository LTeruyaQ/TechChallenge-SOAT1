using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Veiculo
{
    public class EditarVeiculoDto
    {
        [StringLength(8, MinimumLength = 7, ErrorMessage = "A placa deve ter entre 7 e 8 caracteres.")]
        [RegularExpression(@"^[A-Za-z]{3}\d{1}[A-Za-z]{1}\d{2}$|^[A-Za-z]{3}\d{4}$",
            ErrorMessage = "Formato de placa inválido. Use o formato AAA1A11 ou AAA1111.")]
        public string Placa { get; set; }

        [StringLength(50, ErrorMessage = "A marca deve ter no máximo 50 caracteres.")]
        public string Marca { get; set; }

        [StringLength(50, ErrorMessage = "O modelo deve ter no máximo 50 caracteres.")]
        public string Modelo { get; set; }

        [StringLength(30, ErrorMessage = "A cor deve ter no máximo 30 caracteres.")]
        public string Cor { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "O ano deve ter 4 dígitos.")]
        [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100.")]
        public string Ano { get; set; }

        [StringLength(500, ErrorMessage = "As anotações devem ter no máximo 500 caracteres.")]
        public string Anotacoes { get; set; }

        public Guid? ClienteId { get; set; } // Opcional na edição
    }
}