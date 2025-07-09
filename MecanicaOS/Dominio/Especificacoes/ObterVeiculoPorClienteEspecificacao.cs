using System.Linq.Expressions;
using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes
{
    public class ObterVeiculoPorClienteEspecificacao : IEspecificacao<Veiculo>
    {
        private readonly Guid _clienteId;

        public ObterVeiculoPorClienteEspecificacao(Guid clienteId)
        {
            _clienteId = clienteId;
        }

        public Expression<Func<Veiculo, bool>> Expressao =>
            v => v.ClienteId == _clienteId;
    }
}
