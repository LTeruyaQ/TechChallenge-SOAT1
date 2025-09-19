﻿using MediatR;

namespace Infraestrutura.Notificacoes.OS;

public class OrdemServicoFinalizadaEvent(Guid ordemServicoId) : INotification
{
    public Guid OrdemServicoId { get; } = ordemServicoId;
}