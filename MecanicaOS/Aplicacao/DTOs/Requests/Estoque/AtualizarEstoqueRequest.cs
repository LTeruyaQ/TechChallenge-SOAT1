namespace Aplicacao.DTOs.Requests.Estoque
{
    public class AtualizarEstoqueRequest
    {
        public string? Insumo { get; set; } = default!;
        public string? Descricao { get; set; }
        public double? Preco { get; set; }
        public int? QuantidadeDisponivel { get; set; }
        public int? QuantidadeMinima { get; set; }
    }
}
