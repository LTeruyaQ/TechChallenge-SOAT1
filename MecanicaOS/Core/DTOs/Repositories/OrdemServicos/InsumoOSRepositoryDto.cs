using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Estoque;

namespace Core.DTOs.Repositories.OrdemServicos;

public class InsumoOSRepositoryDto : RepositoryDto
{
    public Guid OrdemServicoId { get; set; }
    public OrdemServicoRepositoryDto OrdemServico { get; set; } = null!;
    public Guid EstoqueId { get; set; }
    public EstoqueRepositoryDto Estoque { get; set; } = null!;
    public int Quantidade { get; set; }
}