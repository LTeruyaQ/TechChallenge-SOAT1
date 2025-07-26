using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.OrdemServico;

public class ObterOrdemServicoPorIdComInsumosEspecificacao : EspecificacaoBase<Entidades.OrdemServico>
{
    private readonly Guid _id;

    public ObterOrdemServicoPorIdComInsumosEspecificacao(Guid id)
    {
        _id = id;
        AdicionarInclusao(os => os.InsumosOS);
    }

    public override Expression<Func<Entidades.OrdemServico, bool>> Expressao => os => os.Id == _id;
}
