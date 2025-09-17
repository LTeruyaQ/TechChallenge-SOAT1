using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.ObterVeiculoPorCliente
{
    public class ObterVeiculoPorClienteHandler : UseCasesAbstrato<ObterVeiculoPorClienteHandler, Veiculo>
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public ObterVeiculoPorClienteHandler(
            IVeiculoGateway veiculoGateway,
            ILogServico<ObterVeiculoPorClienteHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway ?? throw new ArgumentNullException(nameof(veiculoGateway));
        }

        public async Task<ObterVeiculoPorClienteResponse> Handle(ObterVeiculoPorClienteUseCase query)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, query.ClienteId);

                var veiculos = await _veiculoGateway.ObterVeiculoPorClienteAsync(query.ClienteId);
                LogFim(metodo, veiculos);

                return new ObterVeiculoPorClienteResponse { Veiculos = veiculos };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
