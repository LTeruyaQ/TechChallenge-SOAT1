using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.UseCases;
using Core.UseCases.OrdensServico.AceitarOrcamento;
using Core.UseCases.OrdensServico.AtualizarOrdemServico;
using Core.UseCases.OrdensServico.CadastrarOrdemServico;
using Core.UseCases.OrdensServico.ObterOrdemServico;
using Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus;
using Core.UseCases.OrdensServico.ObterTodosOrdensServico;
using Core.UseCases.OrdensServico.RecusarOrcamento;

namespace Core.UseCases.OrdensServico
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IOrdemServicoUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class OrdemServicoUseCasesFacade : IOrdemServicoUseCases
    {
        private readonly CadastrarOrdemServicoHandler _cadastrarOrdemServicoHandler;
        private readonly AtualizarOrdemServicoHandler _atualizarOrdemServicoHandler;
        private readonly ObterOrdemServicoHandler _obterOrdemServicoHandler;
        private readonly ObterTodosOrdensServicoHandler _obterTodosOrdensServicoHandler;
        private readonly ObterOrdemServicoPorStatusHandler _obterOrdemServicoPorStatusHandler;
        private readonly AceitarOrcamentoHandler _aceitarOrcamentoHandler;
        private readonly RecusarOrcamentoHandler _recusarOrcamentoHandler;

        public OrdemServicoUseCasesFacade(
            CadastrarOrdemServicoHandler cadastrarOrdemServicoHandler,
            AtualizarOrdemServicoHandler atualizarOrdemServicoHandler,
            ObterOrdemServicoHandler obterOrdemServicoHandler,
            ObterTodosOrdensServicoHandler obterTodosOrdensServicoHandler,
            ObterOrdemServicoPorStatusHandler obterOrdemServicoPorStatusHandler,
            AceitarOrcamentoHandler aceitarOrcamentoHandler,
            RecusarOrcamentoHandler recusarOrcamentoHandler)
        {
            _cadastrarOrdemServicoHandler = cadastrarOrdemServicoHandler ?? throw new ArgumentNullException(nameof(cadastrarOrdemServicoHandler));
            _atualizarOrdemServicoHandler = atualizarOrdemServicoHandler ?? throw new ArgumentNullException(nameof(atualizarOrdemServicoHandler));
            _obterOrdemServicoHandler = obterOrdemServicoHandler ?? throw new ArgumentNullException(nameof(obterOrdemServicoHandler));
            _obterTodosOrdensServicoHandler = obterTodosOrdensServicoHandler ?? throw new ArgumentNullException(nameof(obterTodosOrdensServicoHandler));
            _obterOrdemServicoPorStatusHandler = obterOrdemServicoPorStatusHandler ?? throw new ArgumentNullException(nameof(obterOrdemServicoPorStatusHandler));
            _aceitarOrcamentoHandler = aceitarOrcamentoHandler ?? throw new ArgumentNullException(nameof(aceitarOrcamentoHandler));
            _recusarOrcamentoHandler = recusarOrcamentoHandler ?? throw new ArgumentNullException(nameof(recusarOrcamentoHandler));
        }

        public async Task<OrdemServico> CadastrarUseCaseAsync(CadastrarOrdemServicoUseCaseDto request)
        {
            var response = await _cadastrarOrdemServicoHandler.Handle(request);
            return response.OrdemServico;
        }

        public async Task<OrdemServico> AtualizarUseCaseAsync(Guid id, AtualizarOrdemServicoUseCaseDto request)
        {
            var response = await _atualizarOrdemServicoHandler.Handle(id, request);
            return response.OrdemServico;
        }

        public async Task<OrdemServico?> ObterPorIdUseCaseAsync(Guid id)
        {
            var response = await _obterOrdemServicoHandler.Handle(id);
            return response.OrdemServico;
        }

        public async Task<IEnumerable<OrdemServico>> ObterTodosUseCaseAsync()
        {
            var response = await _obterTodosOrdensServicoHandler.Handle();
            return response.OrdensServico;
        }

        public async Task<IEnumerable<OrdemServico>> ObterPorStatusUseCaseAsync(StatusOrdemServico status)
        {
            var response = await _obterOrdemServicoPorStatusHandler.Handle(status);
            return response.OrdensServico;
        }

        public async Task AceitarOrcamentoUseCaseAsync(Guid id)
        {
            await _aceitarOrcamentoHandler.Handle(id);
        }

        public async Task RecusarOrcamentoUseCaseAsync(Guid id)
        {
            await _recusarOrcamentoHandler.Handle(id);
        }
    }
}
