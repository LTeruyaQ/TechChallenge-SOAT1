using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterClienteComVeiculoPorIdEspecificacao(Guid clienteId) : EspecificacaoBase<Cliente>
{
    private readonly Guid _clienteId = clienteId;

    public override Expression<Func<Cliente, bool>> Expressao =>
       i => i.Id == _clienteId;

    public override List<Func<IQueryable<Cliente>, IQueryable<Cliente>>> Inclusoes =>
    [
        c => c.Include(c => c.Veiculos)
    ];
}