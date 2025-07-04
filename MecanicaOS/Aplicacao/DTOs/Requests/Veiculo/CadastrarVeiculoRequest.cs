namespace Aplicacao.DTOs.Requests.Veiculo
{
    public class CadastrarVeiculoRequest
    {
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public string Ano { get; set; } = string.Empty;
        public string? Anotacoes { get; set; }
        public Guid ClienteId { get; set; }
    }
}
