using Aplicacao.Servicos.Abstrato;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class OrdemServicoServico : ServicoAbstrato<OrdemServicoServico, OrdemServico>, IOrdemServicoServico
{
    public OrdemServicoServico(ICrudRepositorio<OrdemServico> repositorio, ILogServico<OrdemServicoServico> logServico, IUnidadeDeTrabalho uot) :
        base(repositorio, logServico, uot)
    {
    }
}
