using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Clientes;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Clientes.ObterCliente
{
    public class ObterClienteHandler : UseCasesHandlerAbstrato<ObterClienteHandler>, IObterClienteHandler
    {
        private readonly IClienteGateway _clienteGateway;

        public ObterClienteHandler(
            IClienteGateway clienteGateway,
            ILogServicoGateway<ObterClienteHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _clienteGateway = clienteGateway ?? throw new ArgumentNullException(nameof(clienteGateway));
        }

        public async Task<ObterClienteResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var clienteComVeiculo = await _clienteGateway.ObterPorIdAsync(id);

                if (clienteComVeiculo == null)
                {
                    // Tenta buscar o cliente com veículo
                    clienteComVeiculo = await _clienteGateway.ObterClienteComVeiculoPorIdAsync(id);
                }

                LogFim(metodo, clienteComVeiculo);

                return new ObterClienteResponse { Cliente = clienteComVeiculo };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
