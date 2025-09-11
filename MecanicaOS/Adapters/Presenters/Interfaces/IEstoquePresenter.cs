using Adapters.DTOs.Requests.Estoque;
using Adapters.DTOs.Responses.Estoque;
using Core.DTOs.Estoque;
using Core.Entidades;

namespace Adapters.Presenters.Interfaces
{
    public interface IEstoquePresenter
    {
        CadastrarEstoqueUseCaseDto? ParaUseCaseDto(CadastrarEstoqueRequest request);
        AtualizarEstoqueUseCaseDto? ParaUseCaseDto(AtualizarEstoqueRequest request);
        EstoqueResponse? ParaResponse(Estoque estoque);
        IEnumerable<EstoqueResponse?> ParaResponse(IEnumerable<Estoque> estoques);
    }
}
