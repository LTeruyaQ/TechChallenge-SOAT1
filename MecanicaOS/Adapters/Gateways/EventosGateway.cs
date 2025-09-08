using Core.DTOs.Eventos;
using Core.Interfaces.Gateways;

namespace Adapters.Gateways
{
    public class EventosGateway : IEventosGateway
    {
        public async Task Publicar(OrdemServicoFinalizadaEventDTO eventoDTO)
        {
            throw new NotImplementedException();
        }

        public async Task Publicar(OrdemServicoEmOrcamentoEventDTO eventoDTO)
        {
            throw new NotImplementedException();
        }

        public async Task Publicar(OrdemServicoCanceladaEventDTO eventoDTO)
        {
            throw new NotImplementedException();
        }
    }
}
