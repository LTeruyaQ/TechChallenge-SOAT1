using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Enumeradores;

namespace Aplicacao.DTOs.Responses.OrdemServico;

public class OrdemServicoResponse
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid VeiculoId { get; set; }
    public Guid ServicoId { get; set; }
    public double? Orcamento { get; set; }
    public string? Descricao { get; set; }
    public StatusOrdemServico Status { get; set; }
    public DateTime? DataEnvioOrcamento { get; set; }
    public IEnumerable<InsumoOSResponse>? Insumos { get; set; }
}