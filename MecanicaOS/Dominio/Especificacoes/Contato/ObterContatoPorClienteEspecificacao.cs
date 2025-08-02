using Dominio.Especificacoes.Base;
using System.Linq.Expressions;
using Dominio.Entidades;

namespace Dominio.Especificacoes.Contato;

public class ObterContatoPorClienteEspecificacao : EspecificacaoBase<Entidades.Contato>
{
    private readonly Guid _clienteId;

    public ObterContatoPorClienteEspecificacao(Guid clienteId)
    {
        _clienteId = clienteId;
    }

    public override Expression<Func<Entidades.Contato, bool>> Expressao => 
        c => c.IdCliente == _clienteId;
}
