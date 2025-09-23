using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.ObterTodosOrdensServico
{
    public class ObterTodosOrdensServicoHandler : UseCasesHandlerAbstrato<ObterTodosOrdensServicoHandler>, IObterTodosOrdensServicoHandler
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;

        public ObterTodosOrdensServicoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            ILogServico<ObterTodosOrdensServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
        }

        public async Task<ObterTodosOrdensServicoResponse> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var ordensServico = await _ordemServicoGateway.ObterTodosAsync();
                
                var response = new ObterTodosOrdensServicoResponse { OrdensServico = ordensServico };
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
