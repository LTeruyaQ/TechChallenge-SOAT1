namespace Core.DTOs.Servico
{
    public class EditarServicoUseCaseDto
    {
        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public required decimal? Valor { get; set; }
        public required bool? Disponivel { get; set; }
    }
}
