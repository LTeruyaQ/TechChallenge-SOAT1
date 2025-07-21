using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

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
