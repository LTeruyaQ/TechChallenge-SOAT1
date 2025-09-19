using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Interfaces.Handlers.Clientes;
using Core.Interfaces.UseCases;

namespace Core.UseCases.Clientes
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IClienteUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class ClienteUseCasesFacade : IClienteUseCases
    {
        private readonly ICadastrarClienteHandler _cadastrarClienteHandler;
        private readonly IAtualizarClienteHandler _atualizarClienteHandler;
        private readonly IObterClienteHandler _obterClienteHandler;
        private readonly IObterTodosClientesHandler _obterTodosClientesHandler;
        private readonly IRemoverClienteHandler _removerClienteHandler;
        private readonly IObterClientePorDocumentoHandler _obterClientePorDocumentoHandler;

        public ClienteUseCasesFacade(
            ICadastrarClienteHandler cadastrarClienteHandler,
            IAtualizarClienteHandler atualizarClienteHandler,
            IObterClienteHandler obterClienteHandler,
            IObterTodosClientesHandler obterTodosClientesHandler,
            IRemoverClienteHandler removerClienteHandler,
            IObterClientePorDocumentoHandler obterClientePorDocumentoHandler)
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
            var response = await _atualizarClienteHandler.Handle(id, request);
            return response.Cliente;
        }

        public async Task<Cliente> CadastrarUseCaseAsync(CadastrarClienteUseCaseDto request)
        {
            var response = await _cadastrarClienteHandler.Handle(request);
            return response.Cliente;
        }

        public async Task<Cliente> ObterPorDocumentoUseCaseAsync(string documento)
        {
            var response = await _obterClientePorDocumentoHandler.Handle(documento);
            return response.Cliente;
        }

        public async Task<Cliente> ObterPorIdUseCaseAsync(Guid id)
        {
            var response = await _obterClienteHandler.Handle(id);
            return response.Cliente;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosUseCaseAsync()
        {
            var response = await _obterTodosClientesHandler.Handle();
            return response.Clientes;
        }

        public async Task<bool> RemoverUseCaseAsync(Guid id)
        {
            var response = await _removerClienteHandler.Handle(id);
            return response.Sucesso;
        }
    }
}
