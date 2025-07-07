using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Cliente : Entidade
{
    public string? Nome { get; set; }
    public string? Sexo { get; set; }
    public string? Documento { get; set; }
    public string? DataNascimento { get; set; }
    public string? TipoCliente { get; set; }
    public Endereco? Endereco { get; set; }
    public Contato? Contato { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();

}