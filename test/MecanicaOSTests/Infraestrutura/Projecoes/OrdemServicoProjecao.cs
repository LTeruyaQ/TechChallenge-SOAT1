namespace MecanicaOSTests.Infraestrutura.Projecoes;

public class OrdemServicoProjecao
{
    public Guid Id { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string VeiculoModelo { get; set; } = string.Empty;
    public string ServicoNome { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
