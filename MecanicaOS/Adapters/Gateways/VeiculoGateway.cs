using Core.Entidades;
using Core.Especificacoes.Veiculo;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class VeiculoGateway : IVeiculoGateway
    {
        private readonly IRepositorio<Veiculo> _repositorioVeiculo;

        public VeiculoGateway(IRepositorio<Veiculo> repositorioVeiculo)
        {
            _repositorioVeiculo = repositorioVeiculo;
        }

        public async Task CadastrarAsync(object veiculo)
        {
            await _repositorioVeiculo.CadastrarAsync((Veiculo)veiculo);
        }

        public async Task DeletarAsync(Veiculo veiculo)
        {
            await _repositorioVeiculo.DeletarAsync(veiculo);
        }

        public async Task EditarAsync(object veiculo)
        {
            await _repositorioVeiculo.EditarAsync((Veiculo)veiculo);
        }

        public async Task<Veiculo?> ObterPorIdAsync(Guid id)
        {
            return await _repositorioVeiculo.ObterPorIdAsync(id);
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
        {
            return await _repositorioVeiculo.ObterTodosAsync();
        }

        public async Task<IEnumerable<Veiculo>> ObterVeiculoPorClienteAsync(Guid clienteId)
        {
            var especificacao = new ObterVeiculoPorClienteEspecificacao(clienteId);
            return await _repositorioVeiculo.ListarAsync(especificacao);
        }

        public async Task<IEnumerable<Veiculo>> ObterVeiculoPorPlacaAsync(string placa)
        {
            var especificacao = new ObterVeiculoPorPlacaEspecificacao(placa);
            return await _repositorioVeiculo.ListarAsync(especificacao);
        }
    }
}
