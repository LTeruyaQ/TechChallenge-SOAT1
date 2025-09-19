using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;

namespace Core.Interfaces.UseCases
{
    public interface IInsumoOSUseCases
    {
        Task<IEnumerable<InsumoOS>> CadastrarInsumosUseCaseAsync(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request);
        Task DevolverInsumosAoEstoqueUseCaseAsync(IEnumerable<DevolverInsumoOSRequest> insumosOS);
    }
}