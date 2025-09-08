namespace Core.DTOs.Eventos
{
    public class OrdemServicoCanceladaEventDTO
    {
        public Guid Id { get; set; }

        public OrdemServicoCanceladaEventDTO(Guid id)
        {
            Id = id;
        }
    }
}
