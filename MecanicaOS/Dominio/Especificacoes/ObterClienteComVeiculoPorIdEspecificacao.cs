using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterClienteComVeiculoPorIdEspecificacao : EspecificacaoBase<Cliente>
{
    private readonly Guid _clienteId;

    public ObterClienteComVeiculoPorIdEspecificacao(Guid clienteId)
    {
        _clienteId = clienteId;
        AdicionarInclusao(c => c.Veiculos);
    }

    public override Expression<Func<Cliente, bool>> Expressao =>
       i => i.Id == _clienteId;
}