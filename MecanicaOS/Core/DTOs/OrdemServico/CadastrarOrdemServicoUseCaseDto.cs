namespace Core.DTOs.OrdemServico;

public class CadastrarOrdemServicoUseCaseDto
{
    public Guid ClienteId { get; set; }
    public Guid VeiculoId { get; set; }
    public Guid ServicoId { get; set; }
    public string? Descricao { get; set; }
}