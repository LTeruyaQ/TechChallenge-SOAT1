using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterOrdemServicoPorIdComInsumosEspecificacao : EspecificacaoBase<OrdemServico>
{
    private readonly Guid _id;

    public ObterOrdemServicoPorIdComInsumosEspecificacao(Guid id)
    {
        _id = id;
        AdicionarInclusao(os => os.InsumosOS);
    }

    public override Expression<Func<OrdemServico, bool>> Expressao => os => os.Id == _id;
}
