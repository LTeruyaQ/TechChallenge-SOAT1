using Adapters.DTOs.Requests.Estoque;
using Adapters.DTOs.Responses.Estoque;
using Adapters.Presenters.Interfaces;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class EstoqueController
    {
        private readonly IEstoqueUseCases _estoqueUseCases;
        private readonly IEstoquePresenter _estoquePresenter;

        public EstoqueController(IEstoqueUseCases estoqueUseCases, IEstoquePresenter estoquePresenter)
        {
            _estoqueUseCases = estoqueUseCases;
            _estoquePresenter = estoquePresenter;
        }

        public async Task<IEnumerable<EstoqueResponse>> ObterTodos()
        {
            return _estoquePresenter.ParaResponse(await _estoqueUseCases.ObterTodosUseCaseAsync());
        }

        public async Task<EstoqueResponse> ObterPorId(Guid id)
        {
            return _estoquePresenter.ParaResponse(await _estoqueUseCases.ObterPorIdUseCaseAsync(id));
        }

        public async Task<EstoqueResponse> Cadastrar(CadastrarEstoqueRequest request)
        {
            return _estoquePresenter.ParaResponse(
                await _estoqueUseCases.CadastrarUseCaseAsync(
                    _estoquePresenter.ParaUseCaseDto(request)));
        }

        public async Task<EstoqueResponse> Atualizar(Guid id, AtualizarEstoqueRequest request)
        {
            return _estoquePresenter.ParaResponse(
                await _estoqueUseCases.AtualizarUseCaseAsync(id,
                    _estoquePresenter.ParaUseCaseDto(request)));
        }

        public async Task<bool> Deletar(Guid id)
        {
            return await _estoqueUseCases.DeletarUseCaseAsync(id);
        }
    }
}
