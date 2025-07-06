using Dominio.Entidades.Abstratos;
using System.Collections.Generic;

namespace Dominio.Entidades;

public class Usuario : Entidade
{
    public Guid IdCliente { get; set; }
    public Cliente? Cliente { get; set; }
    public string? Login { get; set; }
    public string? Senha { get; set; }
    public string? TipoUsuario { get; set; }    

}