namespace Core.DTOs.Eventos
{
    public class OrdemServicoFinalizadaEventDTO
    {
        public Guid OrdemServicoId{ get; set; }

        public OrdemServicoFinalizadaEventDTO(Guid ordemServicoId)
        {
            OrdemServicoId = ordemServicoId;
        }
    }
}
