using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterVeiculoPorClienteEspecificacao : EspecificacaoBase<Veiculo>
    {
        private readonly Guid _clienteId;

        public ObterVeiculoPorClienteEspecificacao(Guid clienteId)
        {
            _clienteId = clienteId;
            AdicionarInclusao(v => v.Cliente);
        }

        public override Expression<Func<Veiculo, bool>> Expressao =>
            v => v.ClienteId == _clienteId;
    }
}