using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.OrdemServico;

public class ObterOrdemServicoPorIdComOrcamentoEspecificacao : EspecificacaoBase<Entidades.OrdemServico>
{
    private Guid id;

    public ObterOrdemServicoPorIdComOrcamentoEspecificacao(Guid id)
    {
        this.id = id;

        AdicionarInclusao(os => os.Orcamento);
    }

    public override Expression<Func<Entidades.OrdemServico, bool>> Expressao => os => os.Id == id;
}