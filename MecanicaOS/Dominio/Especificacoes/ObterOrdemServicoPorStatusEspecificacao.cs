using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterOrdemServicoPorStatusEspecificacao : EspecificacaoBase<OrdemServico>
{
    private readonly StatusOrdemServico _status;

    public ObterOrdemServicoPorStatusEspecificacao(StatusOrdemServico status) => _status = status;

    public override Expression<Func<OrdemServico, bool>> Expressao => os => os.Status == _status;
}