using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Cliente;
using Core.DTOs.Repositories.Servico;
using Core.DTOs.Repositories.Veiculo;
using Core.Enumeradores;

namespace Core.DTOs.Repositories.OrdemServicos;

public class OrdemServicoRepositoryDto : RepositoryDto
{
    public Guid ClienteId { get; set; }
    public ClienteRepositoryDTO Cliente { get; set; } = null!;
    public Guid VeiculoId { get; set; }
    public VeiculoRepositoryDto Veiculo { get; set; } = null!;
    public Guid ServicoId { get; set; }
    public ServicoRepositoryDto Servico { get; set; } = null!;
    public decimal? Orcamento { get; set; }
    public DateTime? DataEnvioOrcamento { get; set; }
    public string? Descricao { get; set; }
    public StatusOrdemServico Status { get; set; }
    public ICollection<InsumoOSRepositoryDto> InsumosOS { get; set; } = [];
}