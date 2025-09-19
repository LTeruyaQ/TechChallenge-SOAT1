using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Interfaces.Handlers.InsumosOS;
using Core.Interfaces.UseCases;

namespace Core.UseCases.InsumosOS
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IInsumoOSUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class InsumoOSUseCasesFacade : IInsumoOSUseCases
    {
        private readonly ICadastrarInsumosHandler _cadastrarInsumosHandler;
        private readonly IDevolverInsumosHandler _devolverInsumosHandler;

        public InsumoOSUseCasesFacade(
            ICadastrarInsumosHandler cadastrarInsumosHandler,
            IDevolverInsumosHandler devolverInsumosHandler)
        {
            _cadastrarInsumosHandler = cadastrarInsumosHandler ?? throw new ArgumentNullException(nameof(cadastrarInsumosHandler));
            _devolverInsumosHandler = devolverInsumosHandler ?? throw new ArgumentNullException(nameof(devolverInsumosHandler));
        }

        public async Task<IEnumerable<InsumoOS>> CadastrarInsumosUseCaseAsync(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request)
        {
            var response = await _cadastrarInsumosHandler.Handle(ordemServicoId, request);
            return response.InsumosOS;
        }

        public async Task DevolverInsumosAoEstoqueUseCaseAsync(IEnumerable<InsumoOS> insumosOS)
        {
            await _devolverInsumosHandler.Handle(insumosOS);
        }
    }
}
