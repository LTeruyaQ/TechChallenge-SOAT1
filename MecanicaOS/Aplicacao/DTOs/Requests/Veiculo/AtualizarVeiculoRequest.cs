using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Requests.Veiculo
{
    [DisplayName("Atualização de Veículo")]
    [SwaggerSchema(Title = "Atualização de Veículo", Description = "Dados para atualização de um veículo existente")]
    public class AtualizarVeiculoRequest
    {
        [StringLength(10, ErrorMessage = "A Placa deve ter no máximo {1} caracteres")]
        [RegularExpression(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$|^[A-Z]{3}[0-9]{4}$",
            ErrorMessage = "Formato de placa inválido. Use o formato: ABC1D23 ou ABC1234")]
        [DisplayName("Placa")]
        [SwaggerSchema(Description = "Placa do veículo (formato: ABC1D23 ou ABC1234)")]
        public string? Placa { get; set; }

        [StringLength(50, ErrorMessage = "A Marca deve ter no máximo {1} caracteres")]
        [DisplayName("Marca")]
        [SwaggerSchema(Description = "Fabricante do veículo")]
        public string? Marca { get; set; }

        [StringLength(100, ErrorMessage = "O Modelo deve ter no máximo {1} caracteres")]
        [DisplayName("Modelo")]
        [SwaggerSchema(Description = "Modelo do veículo")]
        public string? Modelo { get; set; }

        [StringLength(30, ErrorMessage = "A Cor deve ter no máximo {1} caracteres")]
        [DisplayName("Cor")]
        [SwaggerSchema(Description = "Cor predominante do veículo")]
        public string? Cor { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "O Ano deve ter 4 dígitos")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "O Ano deve conter apenas 4 dígitos")]
        [DisplayName("Ano")]
        [SwaggerSchema(Description = "Ano de fabricação do veículo (formato: AAAA)")]
        public string? Ano { get; set; }

        [StringLength(500, ErrorMessage = "As Anotações devem ter no máximo {1} caracteres")]
        [DisplayName("Anotações")]
        [SwaggerSchema(Description = "Observações adicionais sobre o veículo")]
        public string? Anotacoes { get; set; }

        [DisplayName("ID do Cliente")]
        [SwaggerSchema(Description = "Identificador único do cliente proprietário do veículo")]
        public Guid? ClienteId { get; set; }
    }
}
