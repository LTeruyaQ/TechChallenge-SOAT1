namespace Core.DTOs.Eventos
{
    public class OrdemServicoFinalizadaEventDTO
    {
        public Guid Id { get; set; }

        public OrdemServicoFinalizadaEventDTO(Guid id)
        {
            Id = id;
        }
    }
}
