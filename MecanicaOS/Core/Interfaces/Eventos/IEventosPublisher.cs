using Core.DTOs.UseCases.Eventos;
using Core.Entidades;
using Core.Enumeradores;

namespace Core.Interfaces.Eventos
{
    public interface IEventosPublisher
    {
        StatusOrdemServico Status { get; }
        Task PublicarAsync(OrdemServico ordemServico);
    }
}
