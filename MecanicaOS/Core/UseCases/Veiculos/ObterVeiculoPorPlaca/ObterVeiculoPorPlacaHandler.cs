using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.ObterVeiculoPorPlaca
{
    public class ObterVeiculoPorPlacaHandler : UseCasesAbstrato<ObterVeiculoPorPlacaHandler, Veiculo>
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

        public async Task<ObterVeiculoPorPlacaResponse> Handle(ObterVeiculoPorPlacaUseCase query)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, query.Placa);

                var veiculos = await _veiculoGateway.ObterVeiculoPorPlacaAsync(query.Placa);
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
