using Core.DTOs.UseCases.Eventos;
using Core.Entidades;

namespace Core.Interfaces.Gateways
{
    public interface IEventosGateway
    {
        Task Publicar(OrdemServico ordemServico);
    }
}
