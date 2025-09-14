using Core.DTOs.UseCases.Eventos;

namespace Core.Interfaces.Eventos
{
    public interface IEventosPublisher
    {
        public Task Publicar(OrdemServicoFinalizadaEventDTO eventoDTO);
        public Task Publicar(OrdemServicoEmOrcamentoEventDTO eventoDTO);
        public Task Publicar(OrdemServicoCanceladaEventDTO eventoDTO);
    }
}
