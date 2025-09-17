using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Veiculos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.ObterTodosVeiculos
{
    public class ObterTodosVeiculosHandler : UseCasesAbstrato<ObterTodosVeiculosHandler>, IObterTodosVeiculosHandler
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public ObterTodosVeiculosHandler(
            IVeiculoGateway veiculoGateway,
            ILogServico<ObterTodosVeiculosHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway ?? throw new ArgumentNullException(nameof(veiculoGateway));
        }

        public async Task<ObterTodosVeiculosResponse> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var veiculos = await _veiculoGateway.ObterTodosAsync();

                LogFim(metodo, veiculos);

                return new ObterTodosVeiculosResponse { Veiculos = veiculos };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
