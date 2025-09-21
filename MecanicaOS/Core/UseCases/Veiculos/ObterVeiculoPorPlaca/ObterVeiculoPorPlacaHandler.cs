using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Veiculos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.ObterVeiculoPorPlaca
{
    public class ObterVeiculoPorPlacaHandler : UseCasesHandlerAbstrato<ObterVeiculoPorPlacaHandler>, IObterVeiculoPorPlacaHandler
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public ObterVeiculoPorPlacaHandler(
            IVeiculoGateway veiculoGateway,
            ILogServico<ObterVeiculoPorPlacaHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway ?? throw new ArgumentNullException(nameof(veiculoGateway));
        }

        public async Task<ObterVeiculoPorPlacaResponse> Handle(string placa)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, placa);

                var veiculos = await _veiculoGateway.ObterVeiculoPorPlacaAsync(placa);
                var veiculo = veiculos.FirstOrDefault();

                LogFim(metodo, veiculo);

                return new ObterVeiculoPorPlacaResponse { Veiculo = veiculo };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
