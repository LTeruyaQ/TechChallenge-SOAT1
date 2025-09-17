using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Interfaces.UseCases;
using Core.UseCases.InsumosOS.CadastrarInsumos;
using Core.UseCases.InsumosOS.DevolverInsumos;

namespace Core.UseCases.InsumosOS
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IInsumoOSUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class InsumoOSUseCasesFacade : IInsumoOSUseCases
    {
        private readonly CadastrarInsumosHandler _cadastrarInsumosHandler;
        private readonly DevolverInsumosHandler _devolverInsumosHandler;

        public InsumoOSUseCasesFacade(
            CadastrarInsumosHandler cadastrarInsumosHandler,
            DevolverInsumosHandler devolverInsumosHandler)
        {
            _cadastrarInsumosHandler = cadastrarInsumosHandler ?? throw new ArgumentNullException(nameof(cadastrarInsumosHandler));
            _devolverInsumosHandler = devolverInsumosHandler ?? throw new ArgumentNullException(nameof(devolverInsumosHandler));
        }

        public async Task<IEnumerable<InsumoOS>> CadastrarInsumosUseCaseAsync(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request)
        {
            var command = new CadastrarInsumosCommand(ordemServicoId, request);
            var response = await _cadastrarInsumosHandler.Handle(command);
            return response.InsumosOS;
        }

        public async Task DevolverInsumosAoEstoqueUseCaseAsync(IEnumerable<InsumoOS> insumosOS)
        {
            var command = new DevolverInsumosCommand(insumosOS);
            await _devolverInsumosHandler.Handle(command);
        }
    }
}
