using Adapters.DTOs.Requests.Veiculo;
using Core.DTOs.Veiculo;

namespace Adapters.Presenters.Interfaces
{
    public interface IVeiculoPresenter
    {
        CadastrarVeiculoUseCaseDto ParaUseCaseDto(CadastrarVeiculoRequest request);
        AtualizarVeiculoUseCaseDto ParaUseCaseDto(AtualizarVeiculoRequest request);
    }
}
