using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Cliente : Entidade
{
    public string? Nome { get; set; }
    public string? Sexo { get; set; }
    public string? Documento { get; set; }
    public string? DataNascimento { get; set; }
    public string? TipoCliete { get; set; }
    public Endereco? Endereco { get; set; }

}