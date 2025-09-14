using Core.DTOs.Repositories.Veiculo;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Veiculo
{
    public class ObterVeiculoPorPlacaEspecificacao : EspecificacaoBase<VeiculoRepositoryDto>
    {
        private readonly string _placa;

        public ObterVeiculoPorPlacaEspecificacao(string placa)
        {
            _placa = placa;
        }

        public override Expression<Func<VeiculoRepositoryDto, bool>> Expressao =>
            v => v.Placa == _placa;
    }
}
