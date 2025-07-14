using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Contato : Entidade
{
    public Guid IdCliente { get; set; }
    public Cliente? Cliente { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
}