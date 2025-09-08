namespace Core.DTOs.Eventos
{
    public class OrdemServicoEmOrcamentoEventDTO
    {
        public Guid Id { get; set; }

        public OrdemServicoEmOrcamentoEventDTO(Guid id)
        {
            Id = id;
        }
    }
}
