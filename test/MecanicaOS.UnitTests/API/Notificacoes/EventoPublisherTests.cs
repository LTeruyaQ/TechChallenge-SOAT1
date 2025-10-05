using API.Notificacoes;
using API.Notificacoes.OS;
using Core.DTOs.UseCases.Eventos;
using MediatR;
using NSubstitute;
using Xunit;

namespace MecanicaOS.UnitTests.API.Notificacoes
{
    public class EventoPublisherTests
    {
        [Fact]
        public async Task Publicar_OrdemServicoFinalizadaEvent_DeveChamarMediator()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var publisher = new EventoPublisher(mediator);
            var eventoDTO = new OrdemServicoFinalizadaEventDTO(Guid.NewGuid());

            // Act
            await publisher.Publicar(eventoDTO);

            // Assert
            await mediator.Received(1).Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Publicar_OrdemServicoEmOrcamentoEvent_DeveChamarMediator()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var publisher = new EventoPublisher(mediator);
            var eventoDTO = new OrdemServicoEmOrcamentoEventDTO(Guid.NewGuid());

            // Act
            await publisher.Publicar(eventoDTO);

            // Assert
            await mediator.Received(1).Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Publicar_OrdemServicoCanceladaEvent_DeveChamarMediator()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var publisher = new EventoPublisher(mediator);
            var eventoDTO = new OrdemServicoCanceladaEventDTO(Guid.NewGuid());

            // Act
            await publisher.Publicar(eventoDTO);

            // Assert
            await mediator.Received(1).Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
        }
    }
}
