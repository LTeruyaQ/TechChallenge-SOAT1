using Core.Entidades;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Insumo;

public class ObterInsumosPorIdsEOSEspecificacao : EspecificacaoBase<InsumoOS>
{
    private readonly List<Guid> _ids;
    private readonly Guid _ordemServidoId;

    public ObterInsumosPorIdsEOSEspecificacao(Guid ordemServidoId, List<Guid> ids)
    {
        _ids = ids;
        _ordemServidoId = ordemServidoId;
        AdicionarInclusao(i => i.Estoque);
    }

    public override Expression<Func<InsumoOS, bool>> Expressao =>
            i => _ids.Contains(i.Id) && i.OrdemServicoId == _ordemServidoId;
}
