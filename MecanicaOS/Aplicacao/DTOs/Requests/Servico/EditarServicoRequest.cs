using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Aplicacao.DTOs.Requests.Servico
{
    [DisplayName("Edição de Serviço")]
    [SwaggerSchema(Title = "Edição de Serviço", Description = "Dados para edição de um serviço existente")]
    public class EditarServicoRequest
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo {1} caracteres")]
        [DisplayName("Nome do Serviço")]
        [SwaggerSchema(Description = "Nome descritivo do serviço")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório")]
        [StringLength(500, ErrorMessage = "A Descrição deve ter no máximo {1} caracteres")]
        [DisplayName("Descrição")]
        [SwaggerSchema(Description = "Descrição detalhada do serviço")]
        public required string Descricao { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O Valor deve ser maior que zero")]
        [DisplayName("Valor")]
        [SwaggerSchema(Description = "Valor cobrado pelo serviço")]
        public required decimal? Valor { get; set; }

        [DisplayName("Disponível")]
        [SwaggerSchema(Description = "Indica se o serviço está disponível para agendamento")]
        public required bool? Disponivel { get; set; }
    }
}
