using Dominio.Enumeradores;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.OrdemServico;

public class ObterOrdemServicoPorStatusEspecificacao : EspecificacaoBase<Entidades.OrdemServico>
{
    private readonly StatusOrdemServico _status;

    public ObterOrdemServicoPorStatusEspecificacao(StatusOrdemServico status) => _status = status;

    public override Expression<Func<Entidades.OrdemServico, bool>> Expressao => os => os.Status == _status;
}