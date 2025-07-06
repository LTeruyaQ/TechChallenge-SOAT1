using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterVeiculoPorClienteEspecificacao : IEspecificacao<Veiculo>
    {
        private readonly Guid _clienteId;

        public ObterVeiculoPorClienteEspecificacao(Guid clienteId)
        {
            _clienteId = clienteId;
        }

        public Expression<Func<Veiculo, bool>> Expressao => v => true;
        //v => v.ClienteId == _clienteId;
    }
}
