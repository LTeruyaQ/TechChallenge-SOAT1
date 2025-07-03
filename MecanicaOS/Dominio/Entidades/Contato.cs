using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Contato : Entidade
{
    public Cliente cliente { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }    

}