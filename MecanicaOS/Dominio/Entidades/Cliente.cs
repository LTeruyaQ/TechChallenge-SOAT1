using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Cliente : Entidade
{
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
