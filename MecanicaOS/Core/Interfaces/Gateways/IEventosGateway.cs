using Core.DTOs.Eventos;

namespace Core.Interfaces.Gateways
{
    public interface IEventosGateway
    {
        public Task Publicar(OrdemServicoFinalizadaEventDTO eventoDTO);
        public Task Publicar(OrdemServicoEmOrcamentoEventDTO eventoDTO);
        public Task Publicar(OrdemServicoCanceladaEventDTO eventoDTO);
    }
}
