using Adapters.DTOs.Requests.OrdemServico;
using Adapters.DTOs.Responses.OrdemServico;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.OrdemServico;
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
            var useCaseDto = MapearParaCadastrarOrdemServicoUseCaseDto(request);
            var resultado = await _ordemServicoUseCases.CadastrarUseCaseAsync(useCaseDto);
            return _ordemServicoPresenter.ParaResponse(resultado);
        }

        internal CadastrarOrdemServicoUseCaseDto MapearParaCadastrarOrdemServicoUseCaseDto(CadastrarOrdemServicoRequest request)
        {
            if (request is null)
                return null;

            return new CadastrarOrdemServicoUseCaseDto
            {
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                ServicoId = request.ServicoId,
                Descricao = request.Descricao
            };
        }

        public async Task<OrdemServicoResponse> Atualizar(Guid id, AtualizarOrdemServicoRequest request)
        {
            var useCaseDto = MapearParaAtualizarOrdemServicoUseCaseDto(request);
            var resultado = await _ordemServicoUseCases.AtualizarUseCaseAsync(id, useCaseDto);
            return _ordemServicoPresenter.ParaResponse(resultado);
        }

        internal AtualizarOrdemServicoUseCaseDto MapearParaAtualizarOrdemServicoUseCaseDto(AtualizarOrdemServicoRequest request)
        {
            if (request is null)
                return null;

            return new AtualizarOrdemServicoUseCaseDto
            {
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                ServicoId = request.ServicoId,
                Descricao = request.Descricao,
                Status = request.Status
            };
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
