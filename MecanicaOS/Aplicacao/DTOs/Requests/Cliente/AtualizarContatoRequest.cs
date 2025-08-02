using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Aplicacao.DTOs.Requests.Cliente
{
    [DisplayName("Atualização de Contato")]
    [SwaggerSchema(Title = "Atualização de Contato", Description = "Dados para atualização do contato de um cliente")]
    public class AtualizarContatoRequest
    {
        [EmailAddress(ErrorMessage = "O Email informado não é válido")]
        [StringLength(100, ErrorMessage = "O Email deve ter no máximo {1} caracteres")]
        [DisplayName("E-mail")]
        [SwaggerSchema(Description = "Endereço de e-mail do cliente")]
        public string? Email { get; set; }

        [StringLength(15, ErrorMessage = "O Telefone deve ter no máximo {1} caracteres")]
        [RegularExpression(@"^\+?[0-9\s-()]*$", ErrorMessage = "Formato de telefone inválido")]
        [DisplayName("Telefone")]
        [SwaggerSchema(Description = "Número de telefone do cliente")]
        public string? Telefone { get; set; }
    }
}
