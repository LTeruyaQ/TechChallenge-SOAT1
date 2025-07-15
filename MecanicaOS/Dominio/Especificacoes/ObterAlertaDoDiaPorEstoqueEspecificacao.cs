using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterAlertaDoDiaPorEstoqueEspecificacao : EspecificacaoBase<AlertaEstoque>
{
    private readonly Guid _estoqueId;
    private readonly DateTime _data;

    public ObterAlertaDoDiaPorEstoqueEspecificacao(Guid estoqueId, DateTime? data = null)
    {
        _estoqueId = estoqueId;
        _data = (data ?? DateTime.UtcNow).Date;
    }

    public override Expression<Func<AlertaEstoque, bool>> Expressao =>
       a => a.EstoqueId == _estoqueId && a.DataCadastro.Date == _data.Date;
}