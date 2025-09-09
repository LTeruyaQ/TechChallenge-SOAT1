using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.Presenters.Interfaces;
using Core.DTOs.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class InsumoOSController
    {
        private readonly IInsumoOSUseCases _insumoOSUseCases;
        private readonly IOrdemServicoPresenter _ordemServicoPresenter;

        public InsumoOSController(IInsumoOSUseCases insumoOSUseCases, IOrdemServicoPresenter ordemServicoPresenter)
        {
            _insumoOSUseCases = insumoOSUseCases;
            _ordemServicoPresenter = ordemServicoPresenter;
        }

        public async Task<IEnumerable<InsumoOS>> CadastrarInsumos(Guid ordemServicoId, List<CadastrarInsumoOSRequest> requests)
        {
            // Converter os DTOs de request para UseCaseDto
            var useCaseDtos = requests.Select(r => _ordemServicoPresenter.ParaUseCaseDto(r)).ToList();
            
            // Chamar o use case
            return await _insumoOSUseCases.CadastrarInsumosUseCaseAsync(ordemServicoId, useCaseDtos);
        }

        public async Task DevolverInsumosAoEstoque(IEnumerable<InsumoOS> insumosOS)
        {
            await _insumoOSUseCases.DevolverInsumosAoEstoqueUseCaseAsync(insumosOS);
        }
    }
}
