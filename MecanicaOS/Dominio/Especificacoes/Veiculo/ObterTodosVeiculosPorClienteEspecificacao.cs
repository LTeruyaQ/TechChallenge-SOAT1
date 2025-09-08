using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Veiculo
{
    public class ObterTodosVeiculosPorClienteEspecificacao : EspecificacaoBase<Entidades.Veiculo>
    {
        private readonly Guid _clienteId;

        public ObterTodosVeiculosPorClienteEspecificacao(Guid clienteId)
        {
            _clienteId = clienteId;
            AdicionarInclusao(v => v.Cliente);
        }

        public override Expression<Func<Entidades.Veiculo, bool>> Expressao =>
            v => v.ClienteId == _clienteId;
    }
}
