using MediatR;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoEmOrcamentoEvent(Guid ordemServicoId, bool? reenvioDeOrcamento = false) : INotification
{
    public Guid OrdemServicoId { get; } = ordemServicoId;
    public bool ReenvioDeOrcamento { get; } = reenvioDeOrcamento ?? false;
}