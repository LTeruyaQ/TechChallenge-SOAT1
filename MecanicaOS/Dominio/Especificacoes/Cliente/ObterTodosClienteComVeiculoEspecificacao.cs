using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Cliente;

public class ObterTodosClienteComVeiculoEspecificacao : EspecificacaoBase<Entidades.Cliente>
{
    public ObterTodosClienteComVeiculoEspecificacao()
    {
        AdicionarInclusao(c => c.Veiculos);
    }

    public override Expression<Func<Entidades.Cliente, bool>> Expressao => null;
}