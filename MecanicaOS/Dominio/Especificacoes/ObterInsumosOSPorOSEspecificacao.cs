using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterInsumosOSPorOSEspecificacao : EspecificacaoBase<InsumoOS>
{
    private readonly Guid _ordemServicoId;

    public ObterInsumosOSPorOSEspecificacao(Guid ordemServicoId)
    {
        _ordemServicoId = ordemServicoId;
    }

    public override Expression<Func<InsumoOS, bool>> Expressao =>
        i => i.OrdemServicoId == _ordemServicoId;

    public override List<Func<IQueryable<InsumoOS>, IQueryable<InsumoOS>>> Inclusoes =>
    [
        i => i.Include(i => i.Estoque)
    ];
}
