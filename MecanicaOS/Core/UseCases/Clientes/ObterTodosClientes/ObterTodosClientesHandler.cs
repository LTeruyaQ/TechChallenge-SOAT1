using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Clientes;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Clientes.ObterTodosClientes
{
    public class ObterTodosClientesHandler : UseCasesAbstrato<ObterTodosClientesHandler>, IObterTodosClientesHandler
    {
        private readonly IClienteGateway _clienteGateway;

        public ObterTodosClientesHandler(
            IClienteGateway clienteGateway,
            ILogServico<ObterTodosClientesHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _clienteGateway = clienteGateway ?? throw new ArgumentNullException(nameof(clienteGateway));
        }

        public async Task<ObterTodosClientesResponse> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var clientesComVeiculos = await _clienteGateway.ObterTodosClienteComVeiculoAsync();

                LogFim(metodo, clientesComVeiculos);

                return new ObterTodosClientesResponse { Clientes = clientesComVeiculos };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
