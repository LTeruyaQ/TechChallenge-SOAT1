using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Requests.Veiculo
{
    [DisplayName("Cadastro de Veículo")]
    [SwaggerSchema(Title = "Cadastro de Veículo", Description = "Dados necessários para cadastrar um novo veículo")]
    public class CadastrarVeiculoRequest
    {
        [Required(ErrorMessage = "O campo Placa é obrigatório")]
        [StringLength(10, ErrorMessage = "A Placa deve ter no máximo {1} caracteres")]
        [RegularExpression(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$|^[A-Z]{3}[0-9]{4}$",
            ErrorMessage = "Formato de placa inválido. Use o formato: ABC1D23 ou ABC1234")]
        [DisplayName("Placa")]
        [SwaggerSchema(Description = "Placa do veículo (formato: ABC1D23 ou ABC1234)", Nullable = false)]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Marca é obrigatório")]
        [StringLength(50, ErrorMessage = "A Marca deve ter no máximo {1} caracteres")]
        [DisplayName("Marca")]
        [SwaggerSchema(Description = "Fabricante do veículo", Nullable = false)]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Modelo é obrigatório")]
        [StringLength(100, ErrorMessage = "O Modelo deve ter no máximo {1} caracteres")]
        [DisplayName("Modelo")]
        [SwaggerSchema(Description = "Modelo do veículo", Nullable = false)]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Cor é obrigatório")]
        [StringLength(30, ErrorMessage = "A Cor deve ter no máximo {1} caracteres")]
        [DisplayName("Cor")]
        [SwaggerSchema(Description = "Cor predominante do veículo", Nullable = false)]
        public string Cor { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Ano é obrigatório")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "O Ano deve ter 4 dígitos")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "O Ano deve conter apenas 4 dígitos")]
        [DisplayName("Ano")]
        [SwaggerSchema(Description = "Ano de fabricação do veículo (formato: AAAA)", Nullable = false)]
        public string Ano { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "As Anotações devem ter no máximo {1} caracteres")]
        [DisplayName("Anotações")]
        [SwaggerSchema(Description = "Observações adicionais sobre o veículo")]
        public string? Anotacoes { get; set; }

        [Required(ErrorMessage = "O ID do Cliente é obrigatório")]
        [DisplayName("ID do Cliente")]
        [SwaggerSchema(Description = "Identificador único do cliente proprietário do veículo", Nullable = false)]
        public Guid ClienteId { get; set; }
    }
}
