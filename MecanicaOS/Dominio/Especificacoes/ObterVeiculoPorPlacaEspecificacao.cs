using System.Linq.Expressions;
using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes
{
    public class ObterVeiculoPorPlacaEspecificacao : IEspecificacao<Veiculo>
    {
        private readonly string _placa;

        public ObterVeiculoPorPlacaEspecificacao(string placa)
        {
            _placa = placa;
        }

        public Expression<Func<Veiculo, bool>> Expressao =>
            v => v.Placa == _placa;
    }
}
