using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Interfaces.UseCases;
using Core.UseCases.Clientes.AtualizarCliente;
using Core.UseCases.Clientes.CadastrarCliente;
using Core.UseCases.Clientes.ObterCliente;
using Core.UseCases.Clientes.ObterClientePorDocumento;
using Core.UseCases.Clientes.ObterTodosClientes;
using Core.UseCases.Clientes.RemoverCliente;

namespace Core.UseCases.Clientes
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IClienteUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class ClienteUseCasesFacade : IClienteUseCases
    {
        private readonly CadastrarClienteHandler _cadastrarClienteHandler;
        private readonly AtualizarClienteHandler _atualizarClienteHandler;
        private readonly ObterClienteHandler _obterClienteHandler;
        private readonly ObterTodosClientesHandler _obterTodosClientesHandler;
        private readonly RemoverClienteHandler _removerClienteHandler;
        private readonly ObterClientePorDocumentoHandler _obterClientePorDocumentoHandler;

        public ClienteUseCasesFacade(
            CadastrarClienteHandler cadastrarClienteHandler,
            AtualizarClienteHandler atualizarClienteHandler,
            ObterClienteHandler obterClienteHandler,
            ObterTodosClientesHandler obterTodosClientesHandler,
            RemoverClienteHandler removerClienteHandler,
            ObterClientePorDocumentoHandler obterClientePorDocumentoHandler)
        {
            _cadastrarClienteHandler = cadastrarClienteHandler ?? throw new ArgumentNullException(nameof(cadastrarClienteHandler));
            _atualizarClienteHandler = atualizarClienteHandler ?? throw new ArgumentNullException(nameof(atualizarClienteHandler));
            _obterClienteHandler = obterClienteHandler ?? throw new ArgumentNullException(nameof(obterClienteHandler));
            _obterTodosClientesHandler = obterTodosClientesHandler ?? throw new ArgumentNullException(nameof(obterTodosClientesHandler));
            _removerClienteHandler = removerClienteHandler ?? throw new ArgumentNullException(nameof(removerClienteHandler));
            _obterClientePorDocumentoHandler = obterClientePorDocumentoHandler ?? throw new ArgumentNullException(nameof(obterClientePorDocumentoHandler));
        }

        public async Task<Cliente> AtualizarUseCaseAsync(Guid id, AtualizarClienteUseCaseDto request)
        {
            var command = new AtualizarClienteCommand(id, request);
            var response = await _atualizarClienteHandler.Handle(command);
            return response.Cliente;
        }

        public async Task<Cliente> CadastrarUseCaseAsync(CadastrarClienteUseCaseDto request)
        {
            var command = new CadastrarClienteCommand(request);
            var response = await _cadastrarClienteHandler.Handle(command);
            return response.Cliente;
        }

        public async Task<Cliente> ObterPorDocumentoUseCaseAsync(string documento)
        {
            var query = new ObterClientePorDocumentoUseCase(documento);
            var response = await _obterClientePorDocumentoHandler.Handle(query);
            return response.Cliente;
        }

        public async Task<Cliente> ObterPorIdUseCaseAsync(Guid id)
        {
            var query = new ObterClienteUseCase(id);
            var response = await _obterClienteHandler.Handle(query);
            return response.Cliente;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosUseCaseAsync()
        {
            var query = new ObterTodosClientesUseCase();
            var response = await _obterTodosClientesHandler.Handle(query);
            return response.Clientes;
        }

        public async Task<bool> RemoverUseCaseAsync(Guid id)
        {
            var command = new RemoverClienteCommand(id);
            var response = await _removerClienteHandler.Handle(command);
            return response.Sucesso;
        }
    }
}
