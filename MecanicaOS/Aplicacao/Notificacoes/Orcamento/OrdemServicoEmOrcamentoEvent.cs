using MediatR;

namespace Aplicacao.Notificacoes.Orcamento;

public class OrdemServicoEmOrcamentoEvent(Guid ordemServicoId) : INotification
{
    public Guid OrdemServicoId { get; } = ordemServicoId;
}