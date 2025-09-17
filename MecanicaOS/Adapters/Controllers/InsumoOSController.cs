using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
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
            var useCaseDtos = requests.Select(MapearParaCadastrarInsumoOSUseCaseDto).ToList();

            // Chamar o use case
            return await _insumoOSUseCases.CadastrarInsumosUseCaseAsync(ordemServicoId, useCaseDtos);
        }

        internal CadastrarInsumoOSUseCaseDto MapearParaCadastrarInsumoOSUseCaseDto(CadastrarInsumoOSRequest request)
        {
            if (request is null)
                return null;

            return new CadastrarInsumoOSUseCaseDto
            {
                EstoqueId = request.EstoqueId,
                Quantidade = request.Quantidade
            };
        }

        public async Task DevolverInsumosAoEstoque(IEnumerable<InsumoOS> insumosOS)
        {
            await _insumoOSUseCases.DevolverInsumosAoEstoqueUseCaseAsync(insumosOS);
        }
    }
}
