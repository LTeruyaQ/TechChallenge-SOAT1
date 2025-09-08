using Core.Enumeradores;

namespace Core.DTOs.OrdemServico;

public class AtualizarOrdemServicoUseCaseDto
{
    public Guid? ClienteId { get; set; }
    public Guid? VeiculoId { get; set; }
    public Guid? ServicoId { get; set; }
    public string? Descricao { get; set; }
    public StatusOrdemServico? Status { get; set; }
}