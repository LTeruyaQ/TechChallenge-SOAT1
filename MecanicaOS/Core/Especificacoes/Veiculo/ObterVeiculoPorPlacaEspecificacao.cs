using Core.DTOs.Entidades.Veiculo;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Veiculo
{
    public class ObterVeiculoPorPlacaEspecificacao : EspecificacaoBase<VeiculoEntityDto>
    {
        private readonly string _placa;

        public ObterVeiculoPorPlacaEspecificacao(string placa)
        {
            _placa = placa;
        }

        public override Expression<Func<VeiculoEntityDto, bool>> Expressao =>
            v => v.Placa == _placa;
    }
}
