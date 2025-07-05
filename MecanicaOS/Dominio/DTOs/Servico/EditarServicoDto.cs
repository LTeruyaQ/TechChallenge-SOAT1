namespace Dominio.DTOs.Servico
{
    public class EditarServicoDto
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal? Valor { get; set; }
        public bool? Disponivel { get; set; }
    }
}
