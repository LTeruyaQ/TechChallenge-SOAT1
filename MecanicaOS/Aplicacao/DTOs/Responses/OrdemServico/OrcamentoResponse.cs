using Dominio.Enumeradores;

namespace Aplicacao.DTOs.Responses.OrdemServico;

public class OrcamentoResponse
{
    public Guid OrdemServicoId { get; set; }
    public decimal Valor { get; set; }
    public StatusOrcamentoEnum Status { get; set; }
    public DateTime? DataEnvioOrcamento { get; set; }
}