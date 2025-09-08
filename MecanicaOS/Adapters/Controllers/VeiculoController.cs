using Adapters.DTOs.Requests.Veiculo;
using Adapters.DTOs.Responses.Veiculo;
using Adapters.Presenters.Interfaces;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class VeiculoController
    {
        private readonly IVeiculoUseCases _veiculoUseCases;
        private readonly IVeiculoPresenter _veiculoPresenter;

        public VeiculoController(IVeiculoUseCases veiculoUseCases, IVeiculoPresenter veiculoPresenter)
        {
            _veiculoUseCases = veiculoUseCases;
            _veiculoPresenter = veiculoPresenter;
        }

        public async Task<IEnumerable<VeiculoResponse>> ObterTodos()
        {
            return _veiculoPresenter.ParaResponse(await _veiculoUseCases.ObterTodosUseCaseAsync());
        }

        public async Task<VeiculoResponse> ObterPorId(Guid id)
        {
            return _veiculoPresenter.ParaResponse(await _veiculoUseCases.ObterPorIdUseCaseAsync(id));
        }

        public async Task<VeiculoResponse> ObterPorPlaca(string placa)
        {
            return _veiculoPresenter.ParaResponse(await _veiculoUseCases.ObterPorPlacaUseCaseAsync(placa));
        }

        public async Task<IEnumerable<VeiculoResponse>> ObterPorCliente(Guid clienteId)
        {
            return _veiculoPresenter.ParaResponse(await _veiculoUseCases.ObterPorClienteUseCaseAsync(clienteId));
        }

        public async Task<VeiculoResponse> Cadastrar(CadastrarVeiculoRequest request)
        {
            return _veiculoPresenter.ParaResponse(
                await _veiculoUseCases.CadastrarUseCaseAsync(
                    _veiculoPresenter.ParaUseCaseDto(request)));
        }

        public async Task<VeiculoResponse> Atualizar(Guid id, AtualizarVeiculoRequest request)
        {
            return _veiculoPresenter.ParaResponse(
                await _veiculoUseCases.AtualizarUseCaseAsync(id,
                    _veiculoPresenter.ParaUseCaseDto(request)));
        }

        public async Task<bool> Deletar(Guid id)
        {
            return await _veiculoUseCases.DeletarUseCaseAsync(id);
        }
    }
}
