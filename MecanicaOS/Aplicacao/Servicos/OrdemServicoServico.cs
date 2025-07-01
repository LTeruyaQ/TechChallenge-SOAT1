using Aplicacao.Logs.Services;
using Aplicacao.Servicos.Abstrato;
using Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace Aplicacao.Servicos;

public class OrdemServicoServico : ServicoAbstratoLog<OrdemServicoServico>, IOrdemServicoServico
{
    public OrdemServicoServico(ICorrelationIdService correlationIdLog, 
        ILogger<OrdemServicoServico> logger) : base(correlationIdLog, logger)
    {
    }
}
