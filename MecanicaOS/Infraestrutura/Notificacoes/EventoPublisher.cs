using Core.DTOs.Eventos;
using Core.Interfaces.Eventos;
using Infraestrutura.Notificacoes.OS;
using MediatR;

namespace Infraestrutura.Notificacoes
{
    public class EventoPublisher : IEventosPublisher
    {
        private readonly IMediator _mediator;

        public EventoPublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publicar(OrdemServicoFinalizadaEventDTO eventoDTO)
        {
            OrdemServicoFinalizadaEvent evento = new(eventoDTO.OrdemServicoId);
            await _mediator.Publish(evento);
        }

        public async Task Publicar(OrdemServicoEmOrcamentoEventDTO eventoDTO)
        {
            OrdemServicoEmOrcamentoEvent evnto = new(eventoDTO.OrdemServicoId);
            await _mediator.Publish(evnto);
        }

        public async Task Publicar(OrdemServicoCanceladaEventDTO eventoDTO)
        {
            OrdemServicoCanceladaEvent evento = new(eventoDTO.OrdemServicoId);
            await _mediator.Publish(evento);
        }
    }
}
