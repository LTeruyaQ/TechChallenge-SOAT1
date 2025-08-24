using MediatR;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoFinalizadaEvent(Guid ordemServicoId) : INotification
{
    public Guid OrdemServicoId { get; } = ordemServicoId;
}