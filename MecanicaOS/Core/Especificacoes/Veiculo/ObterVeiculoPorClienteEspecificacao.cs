using Core.DTOs.Entidades.Veiculo;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Veiculo
{
    public class ObterVeiculoPorClienteEspecificacao : EspecificacaoBase<VeiculoEntityDto>
    {
        private readonly Guid _clienteId;

        public ObterVeiculoPorClienteEspecificacao(Guid clienteId)
        {
            _clienteId = clienteId;
            AdicionarInclusao(v => v.Cliente);
        }

        public override Expression<Func<VeiculoEntityDto, bool>> Expressao =>
            v => v.ClienteId == _clienteId;
    }
}