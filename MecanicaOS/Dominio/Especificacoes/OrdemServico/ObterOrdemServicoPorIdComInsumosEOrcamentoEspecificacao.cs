using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.OrdemServico;

public class ObterOrdemServicoPorIdComInsumosEOrcamentoEspecificacao : EspecificacaoBase<Entidades.OrdemServico>
{
    private readonly Guid _id;

    public ObterOrdemServicoPorIdComInsumosEOrcamentoEspecificacao(Guid id)
    {
        _id = id;
        AdicionarInclusao(os => os.InsumosOS);
        AdicionarInclusao(os => os.Orcamento);
    }

    public override Expression<Func<Entidades.OrdemServico, bool>> Expressao => os => os.Id == _id;
}
