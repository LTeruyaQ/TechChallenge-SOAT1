using Adapters.DTOs.Requests.Cliente;
using Adapters.DTOs.Responses.Cliente;
using Adapters.Presenters.Interfaces;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class ClienteController
    {
        private readonly IClienteUseCases _clienteUseCases;
        private readonly IClientePresenter _clientePresenter;

        public ClienteController(IClienteUseCases clienteUseCases, IClientePresenter clientePresenter)
        {
            _clienteUseCases = clienteUseCases;
            _clientePresenter = clientePresenter;
        }

        public async Task<IEnumerable<ClienteResponse>> ObterTodos()
        {
            return _clientePresenter.ParaResponse(await _clienteUseCases.ObterTodosUseCaseAsync());
        }

        public async Task<ClienteResponse> ObterPorId(Guid id)
        {
            return _clientePresenter.ParaResponse(await _clienteUseCases.ObterPorIdUseCaseAsync(id));
        }

        public async Task<ClienteResponse> ObterPorDocumento(string documento)
        {
            return _clientePresenter.ParaResponse(await _clienteUseCases.ObterPorDocumentoUseCaseAsync(documento));
        }

        public async Task<ClienteResponse> Cadastrar(CadastrarClienteRequest request)
        {
            return _clientePresenter.ParaResponse(
                await _clienteUseCases.CadastrarUseCaseAsync(
                    _clientePresenter.ParaUseCaseDto(request)));
        }

        public async Task<ClienteResponse> Atualizar(Guid id, AtualizarClienteRequest request)
        {
            return _clientePresenter.ParaResponse(
                await _clienteUseCases.AtualizarUseCaseAsync(id,
                    _clientePresenter.ParaUseCaseDto(request)));
        }

        public async Task<bool> Remover(Guid id)
        {
            return await _clienteUseCases.RemoverUseCaseAsync(id);
        }
    }
}
