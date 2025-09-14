using Core.DTOs.Repositories.Autenticacao;

namespace Core.DTOs.Repositories.Estoque;

public class AlertaEstoqueRepositoryDto : RepositoryDto
{
    public Guid EstoqueId { get; set; }
    public EstoqueRepositoryDto Estoque { get; set; } = null!;
}