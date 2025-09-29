using Adapters.Presenters;
using Core.DTOs.Requests.OrdemServico;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.UseCases.OrdemServico;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class OrdemServicoController : IOrdemServicoController
    {
        private readonly IOrdemServicoUseCases _ordemServicoUseCases;
        private readonly IOrcamentoUseCases _orcamentoUseCases;
        private readonly IOrdemServicoPresenter _ordemServicoPresenter;

        public OrdemServicoController(ICompositionRoot compositionRoot)
        {
            _ordemServicoUseCases = compositionRoot.CriarOrdemServicoUseCases();
            _ordemServicoPresenter = new OrdemServicoPresenter();
            _orcamentoUseCases = compositionRoot.CriarOrcamentoUseCases();
        }

        public async Task<IEnumerable<OrdemServicoResponse>> ObterTodos()
        {
            // Filtra os valores nulos para garantir que o retorno seja IEnumerable<OrdemServicoResponse>
            return _ordemServicoPresenter
                .ParaResponse(await _ordemServicoUseCases.ObterTodosUseCaseAsync())
                .Where(response => response != null)!;
        }

        public async Task<OrdemServicoResponse> ObterPorId(Guid id)
        {
            var ordemServico = await _ordemServicoUseCases.ObterPorIdUseCaseAsync(id);
            if (ordemServico == null)
                throw new InvalidOperationException("Ordem de Serviço não encontrada.");
            var response = _ordemServicoPresenter.ParaResponse(ordemServico);
            if (response == null)
                throw new InvalidOperationException("Erro ao converter Ordem de Serviço para resposta.");
            return response;
        }

        public async Task<IEnumerable<OrdemServicoResponse>> ObterPorStatus(StatusOrdemServico status)
        {
            // Filtra os valores nulos para garantir que o retorno seja IEnumerable<OrdemServicoResponse>
            return _ordemServicoPresenter
                .ParaResponse(await _ordemServicoUseCases.ObterPorStatusUseCaseAsync(status))
                .Where(response => response != null)!;
        }

        public async Task<OrdemServicoResponse> Cadastrar(CadastrarOrdemServicoRequest request)
        {
            var useCaseDto = MapearParaCadastrarOrdemServicoUseCaseDto(request);
            var resultado = await _ordemServicoUseCases.CadastrarUseCaseAsync(useCaseDto);
            var response = _ordemServicoPresenter.ParaResponse(resultado);
            if (response == null)
                throw new InvalidOperationException("O resultado do cadastro não pode ser nulo.");
            return response;
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
            var response = _ordemServicoPresenter.ParaResponse(resultado);
            if (response == null)
                throw new InvalidOperationException("O resultado da atualização não pode ser nulo.");
            return response;
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

        public async Task CalcularOrcamentoAsync(Guid ordemServicoId)
        {
            var ordemServico = await _ordemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoId);

            if (ordemServico == null)
                throw new InvalidOperationException("Ordem de Serviço não encontrada para cálculo de orçamento.");

            var orcamento = _orcamentoUseCases.GerarOrcamentoUseCase(ordemServico);

            await _ordemServicoUseCases.AtualizarUseCaseAsync(ordemServicoId, new AtualizarOrdemServicoUseCaseDto
            {
                Status = StatusOrdemServico.AguardandoAprovacao,
                Orcamento = orcamento,
                DataEnvioOrcamento = DateTime.UtcNow
            });
        }
    }
}
