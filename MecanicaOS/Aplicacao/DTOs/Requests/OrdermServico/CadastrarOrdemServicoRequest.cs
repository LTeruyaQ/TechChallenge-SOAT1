using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Requests.OrdermServico;

public class CadastrarOrdemServicoRequest
{
    [Required]
    public Guid ClienteId { get; set; }

    [Required]
    public Guid VeiculoId { get; set; }
    
    [Required]
    public Guid ServicoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "O valor deve ser no mínimo 1.")]
    public double? Orcamento { get; set; }

    [MaxLength(1000, ErrorMessage = "O campo deve ter no máximo 1000 caracteres.")]
    public string? Descricao { get; set; }
}
