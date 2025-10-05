using Adapters.Gateways;
using Core.DTOs.UseCases.Eventos;
using Core.Interfaces.Eventos;

namespace MecanicaOS.UnitTests.Adapters.Gateways
{
    /// <summary>
    /// Testes para EventosGateway
    /// Importância: Valida publicação de eventos de domínio.
    /// Garante que eventos sejam publicados corretamente para o sistema de mensageria.
    /// </summary>
    public class EventosGatewayTests
    {
        /// <summary>
        /// Verifica se o gateway publica evento de ordem finalizada
        /// Importância: Valida publicação de eventos de conclusão
        /// </summary>
        [Fact]
        public async Task Publicar_OrdemServicoFinalizada_DevePublicarEvento()
        {
            // Arrange
            var publisherMock = Substitute.For<IEventosPublisher>();
            var eventoDto = new OrdemServicoFinalizadaEventDTO(Guid.NewGuid());
            
            publisherMock.Publicar(eventoDto).Returns(Task.CompletedTask);
            
            var gateway = new EventosGateway(publisherMock);
            
            // Act
            await gateway.Publicar(eventoDto);
            
            // Assert
            await publisherMock.Received(1).Publicar(eventoDto);
        }

        /// <summary>
        /// Verifica se o gateway publica evento de ordem em orçamento
        /// Importância: Valida publicação de eventos de orçamento
        /// </summary>
        [Fact]
        public async Task Publicar_OrdemServicoEmOrcamento_DevePublicarEvento()
        {
            // Arrange
            var publisherMock = Substitute.For<IEventosPublisher>();
            var eventoDto = new OrdemServicoEmOrcamentoEventDTO(Guid.NewGuid());
            
            publisherMock.Publicar(eventoDto).Returns(Task.CompletedTask);
            
            var gateway = new EventosGateway(publisherMock);
            
            // Act
            await gateway.Publicar(eventoDto);
            
            // Assert
            await publisherMock.Received(1).Publicar(eventoDto);
        }

        /// <summary>
        /// Verifica se o gateway publica evento de ordem cancelada
        /// Importância: Valida publicação de eventos de cancelamento
        /// </summary>
        [Fact]
        public async Task Publicar_OrdemServicoCancelada_DevePublicarEvento()
        {
            // Arrange
            var publisherMock = Substitute.For<IEventosPublisher>();
            var eventoDto = new OrdemServicoCanceladaEventDTO(Guid.NewGuid());
            
            publisherMock.Publicar(eventoDto).Returns(Task.CompletedTask);
            
            var gateway = new EventosGateway(publisherMock);
            
            // Act
            await gateway.Publicar(eventoDto);
            
            // Assert
            await publisherMock.Received(1).Publicar(eventoDto);
        }
    }
}
