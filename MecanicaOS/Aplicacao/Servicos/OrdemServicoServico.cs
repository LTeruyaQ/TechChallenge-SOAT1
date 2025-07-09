using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class OrdemServicoServico : ServicoAbstrato<OrdemServicoServico, OrdemServico>, IOrdemServicoServico
{
    public OrdemServicoServico(IRepositorio<OrdemServico> repositorio, ILogServico<OrdemServicoServico> logServico, IUnidadeDeTrabalho uot, IMapper mapper) :
        base(repositorio, logServico, uot, mapper)
    {
    }
}
