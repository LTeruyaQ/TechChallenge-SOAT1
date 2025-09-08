using Adapters.DTOs.Requests.Cliente;
using Core.DTOs.Cliente;

namespace Adapters.Presenters.Interfaces
{
    public interface IClientePresenter
    {
        CadastrarClienteUseCaseDto ParaUseCaseDto(CadastrarClienteRequest request);
        AtualizarClienteUseCaseDto ParaUseCaseDto(AtualizarClienteRequest request);
    }
}
