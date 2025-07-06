using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Servico : Entidade
{
    public required string Nome { get; set; }
    public required string Descricao { get; set; }
    public decimal Valor { get; set; }
    public bool Disponivel { get; set; }
    public Servico()
    { }

    public void Atualizar(string nome, string descricao, decimal valor, bool disponivel)
    {

        throw new NotImplementedException();
    }
}