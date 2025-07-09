using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterVeiculoPorClienteEspecificacao : Especificacao<Veiculo>
    {
        private readonly Guid _clienteId;

        public ObterVeiculoPorClienteEspecificacao(Guid clienteId)
        {
            _clienteId = clienteId;
        }

        public override Expression<Func<Veiculo, bool>> Expressao => v => v.ClienteId == _clienteId;
    }
}
