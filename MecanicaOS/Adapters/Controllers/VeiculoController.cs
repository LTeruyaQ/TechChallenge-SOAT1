using Adapters.DTOs.Requests.Veiculo;
using Adapters.DTOs.Responses.Veiculo;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.Veiculo;
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
            var useCaseDto = MapearParaCadastrarVeiculoUseCaseDto(request);
            var resultado = await _veiculoUseCases.CadastrarUseCaseAsync(useCaseDto);
            return _veiculoPresenter.ParaResponse(resultado);
        }
        
        internal CadastrarVeiculoUseCaseDto MapearParaCadastrarVeiculoUseCaseDto(CadastrarVeiculoRequest request)
        {
            if (request is null)
                return null;

            return new CadastrarVeiculoUseCaseDto
            {
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Cor = request.Cor,
                Ano = request.Ano,
                Anotacoes = request.Anotacoes,
                ClienteId = request.ClienteId
            };
        }

        public async Task<VeiculoResponse> Atualizar(Guid id, AtualizarVeiculoRequest request)
        {
            var useCaseDto = MapearParaAtualizarVeiculoUseCaseDto(request);
            var resultado = await _veiculoUseCases.AtualizarUseCaseAsync(id, useCaseDto);
            return _veiculoPresenter.ParaResponse(resultado);
        }
        
        internal AtualizarVeiculoUseCaseDto MapearParaAtualizarVeiculoUseCaseDto(AtualizarVeiculoRequest request)
        {
            if (request is null)
                return null;

            return new AtualizarVeiculoUseCaseDto
            {
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Cor = request.Cor,
                Ano = request.Ano,
                Anotacoes = request.Anotacoes,
                ClienteId = request.ClienteId
            };
        }

        public async Task<bool> Deletar(Guid id)
        {
            return await _veiculoUseCases.DeletarUseCaseAsync(id);
        }
    }
}
