using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterInsumosPorIdsEOSEspecificacao(Guid ordemServidoId, List<Guid> ids) : EspecificacaoBase<InsumoOS>
{
    private readonly List<Guid> _ids = ids;
    private readonly Guid _ordemServidoId = ordemServidoId;

    public override Expression<Func<InsumoOS, bool>> Expressao =>
            i => _ids.Contains(i.Id) && i.OrdemServicoId == _ordemServidoId;

    public override List<Func<IQueryable<InsumoOS>, IQueryable<InsumoOS>>> Inclusoes =>
    [
        i => i.Include(i => i.Estoque)
    ];
}
