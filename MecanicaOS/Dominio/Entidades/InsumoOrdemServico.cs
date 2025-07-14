namespace Dominio.Entidades;

public class InsumoOrdemServico
{
    public Guid OrdemServicoId { get; set; }
    public OrdemServico OrdemServico { get; set; } = null!;
    public Guid EstoqueId { get; set; }
    public Estoque Estoque { get; set; } = null!;
    public int Quantidade { get; set; }
}