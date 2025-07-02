using Dominio.Entidades.Abstratos;

namespace Dominio.Entidades;

public class Veiculo : Entidade
{
    public int Id { get; set; }
    public string Placa { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Cor { get; set; }
    public string Ano { get; set; }
    public string Anotacoes { get; set; }
    public string Data_Atualizacao { get; set; }
    public string Data_Cadastro { get; set; }
    public Cliente Cliente { get; set; }
    public Veiculo()
    { }
}
