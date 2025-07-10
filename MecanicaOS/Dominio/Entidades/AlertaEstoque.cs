using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class AlertaEstoque : Entidade
{
    public Guid EstoqueId { get; set; }
    public Estoque Estoque { get; set; } = null!;
}