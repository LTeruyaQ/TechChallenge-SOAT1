namespace Aplicacao.DTOs.Veiculo
{
    public class CadastrarVeiculoDto
    {
        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string Ano { get; set; }
        public string Anotacoes { get; set; }
        public Cliente Cliente { get; set; }
    }
}
