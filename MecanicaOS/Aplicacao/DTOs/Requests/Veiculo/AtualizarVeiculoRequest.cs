namespace Aplicacao.DTOs.Requests.Veiculo
{
    public class AtualizarVeiculoRequest
    {
        public string? Placa { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Cor { get; set; }
        public string? Ano { get; set; }
        public string? Anotacoes { get; set; }
        public Guid? ClienteId { get; set; }
    }
}
