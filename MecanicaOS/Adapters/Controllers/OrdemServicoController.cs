using Adapters.DTOs.Requests.OrdemServico;
using Adapters.DTOs.Responses.OrdemServico;
using Adapters.Presenters.Interfaces;
using Core.Enumeradores;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class OrdemServicoController
    {
        private readonly IOrdemServicoUseCases _ordemServicoUseCases;
        private readonly IOrdemServicoPresenter _ordemServicoPresenter;

        public OrdemServicoController(IOrdemServicoUseCases ordemServicoUseCases, IOrdemServicoPresenter ordemServicoPresenter)
        {
            _ordemServicoUseCases = ordemServicoUseCases;
            _ordemServicoPresenter = ordemServicoPresenter;
        }

        public async Task<IEnumerable<OrdemServicoResponse>> ObterTodos()
        {
            return _ordemServicoPresenter.ParaResponse(await _ordemServicoUseCases.ObterTodosUseCaseAsync());
        }

        public async Task<OrdemServicoResponse> ObterPorId(Guid id)
        {
            return _ordemServicoPresenter.ParaResponse(await _ordemServicoUseCases.ObterPorIdUseCaseAsync(id));
        }

        public async Task<IEnumerable<OrdemServicoResponse>> ObterPorStatus(StatusOrdemServico status)
        {
            return _ordemServicoPresenter.ParaResponse(await _ordemServicoUseCases.ObterPorStatusUseCaseAsync(status));
        }

        public async Task<OrdemServicoResponse> Cadastrar(CadastrarOrdemServicoRequest request)
        {
            return _ordemServicoPresenter.ParaResponse(
                await _ordemServicoUseCases.CadastrarUseCaseAsync(
                    _ordemServicoPresenter.ParaUseCaseDto(request)));
        }

        public async Task<OrdemServicoResponse> Atualizar(Guid id, AtualizarOrdemServicoRequest request)
        {
            return _ordemServicoPresenter.ParaResponse(
                await _ordemServicoUseCases.AtualizarUseCaseAsync(id,
                    _ordemServicoPresenter.ParaUseCaseDto(request)));
        }

        public async Task AceitarOrcamento(Guid id)
        {
            await _ordemServicoUseCases.AceitarOrcamentoUseCaseAsync(id);
        }

        public async Task RecusarOrcamento(Guid id)
        {
            await _ordemServicoUseCases.RecusarOrcamentoUseCaseAsync(id);
        }
    }
}
