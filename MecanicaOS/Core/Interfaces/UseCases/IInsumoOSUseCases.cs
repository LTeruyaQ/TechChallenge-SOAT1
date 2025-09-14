using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;

namespace Core.Interfaces.UseCases
{
    public interface IInsumoOSUseCases
    {
        Task<IEnumerable<InsumoOS>> CadastrarInsumosUseCaseAsync(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request);
        Task DevolverInsumosAoEstoqueUseCaseAsync(IEnumerable<InsumoOS> insumosOS);
    }
}