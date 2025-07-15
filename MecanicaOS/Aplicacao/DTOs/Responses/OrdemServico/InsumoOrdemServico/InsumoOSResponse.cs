namespace Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;

public class InsumoOSResponse
{
    public Guid OrdemServicoId { get; set; }
    public Guid EstoqueId { get; set; }
    public int Quantidade { get; set; }
}