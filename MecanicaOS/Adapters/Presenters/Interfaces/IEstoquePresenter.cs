using Aplicacao.DTOs.Requests.Estoque;
using Core.DTOs.Estoque;

namespace Adapters.Presenters.Interfaces
{
    public interface IEstoquePresenter
    {
        CadastrarEstoqueUseCaseDto ParaUseCaseDto(CadastrarEstoqueRequest request);
        AtualizarEstoqueUseCaseDto ParaUseCaseDto(AtualizarEstoqueRequest request);
    }
}
