using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Dominio.Enumeradores;

namespace Aplicacao.DTOs.Responses.OrdemServico;

public class OrdemServicoResponse
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid VeiculoId { get; set; }
    public Guid ServicoId { get; set; }
    public OrcamentoResponse? Orcamento { get; set; }
    public string? Descricao { get; set; }
    public StatusOrdemServico Status { get; set; }
    public IEnumerable<InsumoOSResponse>? Insumos { get; set; }
}
