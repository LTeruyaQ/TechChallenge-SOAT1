using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Cliente;

public class ObterClienteComVeiculoPorIdEspecificacao : EspecificacaoBase<Entidades.Cliente>
{
    private readonly Guid _clienteId;

    public ObterClienteComVeiculoPorIdEspecificacao(Guid clienteId)
    {
        _clienteId = clienteId;
        AdicionarInclusao(c => c.Veiculos);
    }

    public override Expression<Func<Entidades.Cliente, bool>> Expressao =>
       i => i.Id == _clienteId;
}