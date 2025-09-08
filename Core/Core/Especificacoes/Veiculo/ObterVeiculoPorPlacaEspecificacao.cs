using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Veiculo
{
    public class ObterVeiculoPorPlacaEspecificacao : EspecificacaoBase<Entidades.Veiculo>
    {
        private readonly string _placa;

        public ObterVeiculoPorPlacaEspecificacao(string placa)
        {
            _placa = placa;
        }

        public override Expression<Func<Entidades.Veiculo, bool>> Expressao =>
            v => v.Placa == _placa;
    }
}
