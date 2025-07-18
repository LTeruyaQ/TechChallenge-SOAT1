using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoFinalizadaHandler(IRepositorio<OrdemServico> ordemServicoRepositorio, ILogServico<OrdemServicoFinalizadaHandler> logServico) : INotificationHandler<OrdemServicoFinalizadaEvent>
{
    private readonly IRepositorio<OrdemServico> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly ILogServico<OrdemServicoFinalizadaHandler> _logServico = logServico;

    public async Task Handle(OrdemServicoFinalizadaEvent notification, CancellationToken cancellationToken)
    {
        var os = await _ordemServicoRepositorio.ObterPorIdAsync(notification.OrdemServicoId);

        if (os is null) return;

        //TODO: notificar cliente que o veículo está pronto
    }
}