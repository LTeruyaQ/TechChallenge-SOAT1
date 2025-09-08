using Core.DTOs.Eventos;
using Core.Interfaces.Eventos;
using Core.Interfaces.Gateways;

namespace Adapters.Gateways
{
    public class EventosGateway : IEventosGateway
    {
        private readonly IEventosPublisher _eventosPublisher;

        public EventosGateway(IEventosPublisher eventosPublisher)
        {
            _eventosPublisher = eventosPublisher;
        }

        public async Task Publicar(OrdemServicoFinalizadaEventDTO eventoDTO)
        {
            await _eventosPublisher.Publicar(eventoDTO);
        }

        public async Task Publicar(OrdemServicoEmOrcamentoEventDTO eventoDTO)
        {
            await _eventosPublisher.Publicar(eventoDTO);
        }

        public async Task Publicar(OrdemServicoCanceladaEventDTO eventoDTO)
        {
            await _eventosPublisher.Publicar(eventoDTO);
        }
    }
}
