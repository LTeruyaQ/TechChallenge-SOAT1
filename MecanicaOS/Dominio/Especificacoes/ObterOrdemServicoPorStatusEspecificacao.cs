using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterOrdemServicoPorStatusEspecificacao : IEspecificacao<OrdemServico>
{
    private readonly StatusOrdemServico _status;

    public ObterOrdemServicoPorStatusEspecificacao(StatusOrdemServico status)
    {
        _status = status;
    }

    public Expression<Func<OrdemServico, bool>> Expressao => os => os.Status == _status;
}