using Core.DTOs.Repositories.Autenticacao;

namespace Core.DTOs.Repositories.Servico;

public class ServicoRepositoryDto : RepositoryDto
{
    public required string Nome { get; set; }
    public required string Descricao { get; set; }
    public decimal Valor { get; set; }
    public bool Disponivel { get; set; }
}