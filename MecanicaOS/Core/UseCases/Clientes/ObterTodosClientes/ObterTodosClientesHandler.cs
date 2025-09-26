using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Clientes;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Clientes.ObterTodosClientes
{
    public class ObterTodosClientesHandler : UseCasesHandlerAbstrato<ObterTodosClientesHandler>, IObterTodosClientesHandler
    {
        private readonly IClienteGateway _clienteGateway;

        public ObterTodosClientesHandler(
            IClienteGateway clienteGateway,
            ILogServicoGateway<ObterTodosClientesHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
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
