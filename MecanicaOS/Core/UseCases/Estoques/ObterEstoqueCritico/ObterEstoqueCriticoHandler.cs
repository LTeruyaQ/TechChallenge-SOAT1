using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Estoques;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Estoques.ObterEstoqueCritico
{
    public class ObterEstoqueCriticoHandler : UseCasesHandlerAbstrato<ObterEstoqueCriticoHandler>, IObterEstoqueCriticoHandler
    {
        private readonly IEstoqueGateway _estoqueGateway;

        public ObterEstoqueCriticoHandler(
            IEstoqueGateway estoqueGateway,
            ILogServicoGateway<ObterEstoqueCriticoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
        }

        public async Task<ObterEstoqueCriticoResponse> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var estoquesCriticos = await _estoqueGateway.ObterEstoqueCriticoAsync();

                LogFim(metodo, estoquesCriticos);

                return new ObterEstoqueCriticoResponse { EstoquesCriticos = estoquesCriticos };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
