namespace Core.DTOs.Eventos
{
    public class OrdemServicoEmOrcamentoEventDTO
    {
        public Guid OrdemServicoId { get; set; }

        public OrdemServicoEmOrcamentoEventDTO(Guid ordemServicoId)
        {
            OrdemServicoId = ordemServicoId;
        }
    }
}
