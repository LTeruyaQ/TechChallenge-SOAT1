using Adapters.DTOs.Requests.Cliente;
using Adapters.DTOs.Responses.Cliente;
using Core.DTOs.Cliente;
using Core.Entidades;

namespace Adapters.Presenters.Interfaces
{
    public interface IClientePresenter
    {
        CadastrarClienteUseCaseDto? ParaUseCaseDto(CadastrarClienteRequest request);
        AtualizarClienteUseCaseDto? ParaUseCaseDto(AtualizarClienteRequest request);
        ClienteResponse? ParaResponse(Cliente cliente);
        IEnumerable<ClienteResponse?> ParaResponse(IEnumerable<Cliente> clientes);
    }
}
