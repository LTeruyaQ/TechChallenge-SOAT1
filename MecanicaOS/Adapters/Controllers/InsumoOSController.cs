using Adapters.Presenters;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Interfaces.Controllers;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class InsumoOSController : IInsumoOSController
    {
        private readonly IInsumoOSUseCases _insumoOSUseCases;
        private readonly IInsumoPresenter _insumoPresenter;

        public InsumoOSController(ICompositionRoot compositionRoot)
        {
            _insumoOSUseCases = compositionRoot.CriarInsumoOSUseCases();
            _insumoPresenter = new InsumoOSPresenter();
        }

        public async Task<IEnumerable<InsumoOS>> CadastrarInsumos(Guid ordemServicoId, IEnumerable<CadastrarInsumoOSRequest> requests)
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

        public async Task DevolverInsumosAoEstoque(IEnumerable<DevolverInsumoOSRequest> insumosOS)
        {
            await _insumoOSUseCases.DevolverInsumosAoEstoqueUseCaseAsync(insumosOS);
        }
    }
}
