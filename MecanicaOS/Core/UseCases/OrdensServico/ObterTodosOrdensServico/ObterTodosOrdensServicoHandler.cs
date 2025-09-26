using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.ObterTodosOrdensServico
{
    public class ObterTodosOrdensServicoHandler : UseCasesHandlerAbstrato<ObterTodosOrdensServicoHandler>, IObterTodosOrdensServicoHandler
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;

        public ObterTodosOrdensServicoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            ILogGateway<ObterTodosOrdensServicoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
        }

        public async Task<IEnumerable<OrdemServico>> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var ordensServico = await _ordemServicoGateway.ObterTodosAsync();

                LogFim(metodo, ordensServico);

                return ordensServico;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
