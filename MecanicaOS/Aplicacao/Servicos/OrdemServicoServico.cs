using Aplicacao.Interfaces;
using Aplicacao.Servicos.Abstrato;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class OrdemServicoServico : ServicoAbstrato<OrdemServicoServico>, IOrdemServicoServico
{
    public OrdemServicoServico(ILogServico<OrdemServicoServico> logServico, IUnidadeDeTrabalho uot) : base(logServico, uot)
    {
    }
}
