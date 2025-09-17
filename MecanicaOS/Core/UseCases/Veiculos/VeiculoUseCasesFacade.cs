using Core.DTOs.UseCases.Veiculo;
using Core.Entidades;
using Core.Interfaces.UseCases;
using Core.UseCases.Veiculos.AtualizarVeiculo;
using Core.UseCases.Veiculos.CadastrarVeiculo;
using Core.UseCases.Veiculos.DeletarVeiculo;
using Core.UseCases.Veiculos.ObterTodosVeiculos;
using Core.UseCases.Veiculos.ObterVeiculo;
using Core.UseCases.Veiculos.ObterVeiculoPorCliente;
using Core.UseCases.Veiculos.ObterVeiculoPorPlaca;

namespace Core.UseCases.Veiculos
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IVeiculoUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class VeiculoUseCasesFacade : IVeiculoUseCases
    {
        private readonly CadastrarVeiculoHandler _cadastrarVeiculoHandler;
        private readonly AtualizarVeiculoHandler _atualizarVeiculoHandler;
        private readonly ObterVeiculoHandler _obterVeiculoHandler;
        private readonly ObterTodosVeiculosHandler _obterTodosVeiculosHandler;
        private readonly ObterVeiculoPorClienteHandler _obterVeiculoPorClienteHandler;
        private readonly ObterVeiculoPorPlacaHandler _obterVeiculoPorPlacaHandler;
        private readonly DeletarVeiculoHandler _deletarVeiculoHandler;

        public VeiculoUseCasesFacade(
            CadastrarVeiculoHandler cadastrarVeiculoHandler,
            AtualizarVeiculoHandler atualizarVeiculoHandler,
            ObterVeiculoHandler obterVeiculoHandler,
            ObterTodosVeiculosHandler obterTodosVeiculosHandler,
            ObterVeiculoPorClienteHandler obterVeiculoPorClienteHandler,
            ObterVeiculoPorPlacaHandler obterVeiculoPorPlacaHandler,
            DeletarVeiculoHandler deletarVeiculoHandler)
        {
            _cadastrarVeiculoHandler = cadastrarVeiculoHandler ?? throw new ArgumentNullException(nameof(cadastrarVeiculoHandler));
            _atualizarVeiculoHandler = atualizarVeiculoHandler ?? throw new ArgumentNullException(nameof(atualizarVeiculoHandler));
            _obterVeiculoHandler = obterVeiculoHandler ?? throw new ArgumentNullException(nameof(obterVeiculoHandler));
            _obterTodosVeiculosHandler = obterTodosVeiculosHandler ?? throw new ArgumentNullException(nameof(obterTodosVeiculosHandler));
            _obterVeiculoPorClienteHandler = obterVeiculoPorClienteHandler ?? throw new ArgumentNullException(nameof(obterVeiculoPorClienteHandler));
            _obterVeiculoPorPlacaHandler = obterVeiculoPorPlacaHandler ?? throw new ArgumentNullException(nameof(obterVeiculoPorPlacaHandler));
            _deletarVeiculoHandler = deletarVeiculoHandler ?? throw new ArgumentNullException(nameof(deletarVeiculoHandler));
        }

        public async Task<Veiculo> AtualizarUseCaseAsync(Guid id, AtualizarVeiculoUseCaseDto request)
        {
            var command = new AtualizarVeiculoCommand(id, request);
            var response = await _atualizarVeiculoHandler.Handle(command);
            return response.Veiculo;
        }

        public async Task<Veiculo> CadastrarUseCaseAsync(CadastrarVeiculoUseCaseDto request)
        {
            var command = new CadastrarVeiculoCommand(request);
            var response = await _cadastrarVeiculoHandler.Handle(command);
            return response.Veiculo;
        }

        public async Task<Veiculo> ObterPorIdUseCaseAsync(Guid id)
        {
            var query = new ObterVeiculoUseCase(id);
            var response = await _obterVeiculoHandler.Handle(query);
            return response.Veiculo;
        }

        public async Task<IEnumerable<Veiculo>> ObterPorClienteUseCaseAsync(Guid clienteId)
        {
            var query = new ObterVeiculoPorClienteUseCase(clienteId);
            var response = await _obterVeiculoPorClienteHandler.Handle(query);
            return response.Veiculos;
        }

        public async Task<Veiculo?> ObterPorPlacaUseCaseAsync(string placa)
        {
            var query = new ObterVeiculoPorPlacaUseCase(placa);
            var response = await _obterVeiculoPorPlacaHandler.Handle(query);
            return response.Veiculo;
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosUseCaseAsync()
        {
            var query = new ObterTodosVeiculosUseCase();
            var response = await _obterTodosVeiculosHandler.Handle(query);
            return response.Veiculos;
        }

        public async Task<bool> DeletarUseCaseAsync(Guid id)
        {
            var command = new DeletarVeiculoCommand(id);
            var response = await _deletarVeiculoHandler.Handle(command);
            return response.Sucesso;
        }
    }
}
