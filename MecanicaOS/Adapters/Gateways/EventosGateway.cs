using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Eventos;
using Core.Interfaces.Gateways;

namespace Adapters.Gateways
{
    public class EventosGateway : IEventosGateway
    {
        private readonly Dictionary<StatusOrdemServico, IEventosPublisher> _handlers;

        public EventosGateway(IEnumerable<IEventosPublisher> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.Status);

        }

        public async Task Publicar(OrdemServico ordemServico)
        {
            if (_handlers.TryGetValue(ordemServico.Status, out var handler))
                await handler.PublicarAsync(ordemServico);
        }
    }
}
