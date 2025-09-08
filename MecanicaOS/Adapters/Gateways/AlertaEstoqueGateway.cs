using Core.Entidades;
using Core.Especificacoes.Estoque;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class AlertaEstoqueGateway : IAlertaEstoqueGateway
    {
        private readonly IRepositorio<AlertaEstoque> _repositorioAlertaEstoque;

        public AlertaEstoqueGateway(IRepositorio<AlertaEstoque> repositorioAlertaEstoque)
        {
            _repositorioAlertaEstoque = repositorioAlertaEstoque;
        }

        public async Task CadastrarVariosAsync(IEnumerable<AlertaEstoque> alertas)
        {
            await _repositorioAlertaEstoque.CadastrarVariosAsync(alertas);
        }

        public async Task<IEnumerable<AlertaEstoque>> ObterAlertaDoDiaPorEstoqueAsync(Guid insumoId, DateTime dataAtual)
        {
            return await _repositorioAlertaEstoque.ListarAsync(
                new ObterAlertaDoDiaPorEstoqueEspecificacao(
                    insumoId,
                    dataAtual));
        }
    }
}
