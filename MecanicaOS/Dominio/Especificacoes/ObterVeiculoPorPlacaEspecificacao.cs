using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterVeiculoPorPlacaEspecificacao : EspecificacaoBase<Veiculo>
    {
        private readonly string _placa;

        public ObterVeiculoPorPlacaEspecificacao(string placa)
        {
            _placa = placa;
        }

        public override Expression<Func<Veiculo, bool>> Expressao =>
            v => v.Placa == _placa;
    }
}
