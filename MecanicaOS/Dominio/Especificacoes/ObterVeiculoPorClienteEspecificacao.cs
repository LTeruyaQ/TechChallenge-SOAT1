using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterVeiculoPorClienteEspecificacao : EspecificacaoBase<Veiculo>
    {
        private readonly Guid _clienteId;

        public ObterVeiculoPorClienteEspecificacao(Guid clienteId)
        {
            _clienteId = clienteId;
        }

        public override Expression<Func<Veiculo, bool>> Expressao =>
            v => v.ClienteId == _clienteId;

        public override List<Func<IQueryable<Veiculo>, IQueryable<Veiculo>>> Inclusoes =>
        [
            i => i.Include(i => i.Cliente)
        ];
    }
}