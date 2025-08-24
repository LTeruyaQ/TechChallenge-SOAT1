using MediatR;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoCanceladaEvent(Guid ordemServicoId) : INotification
{
    public Guid OrdemServicoId { get; } = ordemServicoId;
}