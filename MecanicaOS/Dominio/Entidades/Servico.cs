using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Servico : Entidade
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public bool Disponivel { get; set; }
    public Servico()
    {}
}