using Adapters.DTOs.Requests.Veiculo;
using Adapters.DTOs.Responses.Veiculo;
using Core.DTOs.UseCases.Veiculo;
using Core.Entidades;

namespace Adapters.Presenters.Interfaces
{
    public interface IVeiculoPresenter
    {
        CadastrarVeiculoUseCaseDto? ParaUseCaseDto(CadastrarVeiculoRequest request);
        AtualizarVeiculoUseCaseDto? ParaUseCaseDto(AtualizarVeiculoRequest request);
        VeiculoResponse? ParaResponse(Veiculo veiculo);
        IEnumerable<VeiculoResponse?> ParaResponse(IEnumerable<Veiculo> veiculos);
    }
}
