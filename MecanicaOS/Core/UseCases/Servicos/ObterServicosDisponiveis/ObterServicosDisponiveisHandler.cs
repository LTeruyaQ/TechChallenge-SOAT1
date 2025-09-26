using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.ObterServicosDisponiveis
{
    public class ObterServicosDisponiveisHandler : UseCasesHandlerAbstrato<ObterServicosDisponiveisHandler>, IObterServicosDisponiveisHandler
    {
        private readonly IServicoGateway _servicoGateway;

        public ObterServicosDisponiveisHandler(
            IServicoGateway servicoGateway,
            ILogServicoGateway<ObterServicosDisponiveisHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<ObterServicosDisponiveisResponse> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var servicos = await _servicoGateway.ObterServicoDisponivelAsync();

                var response = new ObterServicosDisponiveisResponse { Servicos = servicos };
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
