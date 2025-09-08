using Core.Entidades;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Insumo;

public class ObterInsumosOSPorOSEspecificacao : EspecificacaoBase<InsumoOS>
{
    private readonly Guid _ordemServicoId;

    public ObterInsumosOSPorOSEspecificacao(Guid ordemServicoId)
    {
        _ordemServicoId = ordemServicoId;
        AdicionarInclusao(i => i.Estoque);
    }

    public override Expression<Func<InsumoOS, bool>> Expressao =>
        i => i.OrdemServicoId == _ordemServicoId;
}
