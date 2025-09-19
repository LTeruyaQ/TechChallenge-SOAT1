using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;

namespace Core.Interfaces.UseCases
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IInsumoOSUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public interface IInsumoOSUseCases
    {
        Task<IEnumerable<InsumoOS>> CadastrarInsumosUseCaseAsync(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request);
        Task DevolverInsumosAoEstoqueUseCaseAsync(IEnumerable<DevolverInsumoOSRequest> insumosOS);
    }
}