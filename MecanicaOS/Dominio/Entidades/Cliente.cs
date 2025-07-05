using Dominio.Entidades.Abstratos;
using System.Collections.Generic;

namespace Dominio.Entidades;

public class Cliente : Entidade
{
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
