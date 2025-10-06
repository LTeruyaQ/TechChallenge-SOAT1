using API.Notificacoes.OS;
using Core.DTOs.UseCases.Eventos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Eventos;
using MediatR;

namespace API.Notificacoes
{
    public class EventoPublisher : IEventosPublisher
    {
        private readonly IMediator _mediator;

        public EventoPublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public StatusOrdemServico Status => throw new NotImplementedException();

        public Task PublicarAsync(OrdemServico ordemServico)
        {
            throw new NotImplementedException();
        }
    }
}
