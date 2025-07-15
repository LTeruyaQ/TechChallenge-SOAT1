using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;

public class AtualizarInsumoOSRequest
{
    public Guid? OrdemServicoId { get; set; }
    
    public Guid? EstoqueId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "O valor deve ser no mínimo 1.")]
    public int? Quantidade { get; set; }
}