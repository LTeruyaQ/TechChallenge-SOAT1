using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.ObterServicosDisponiveis
{
    public class ObterServicosDisponiveisHandler : UseCasesAbstrato<ObterServicosDisponiveisHandler>, IObterServicosDisponiveisHandler
    {
        private readonly IServicoGateway _servicoGateway;

        public ObterServicosDisponiveisHandler(
            IServicoGateway servicoGateway,
            ILogServico<ObterServicosDisponiveisHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
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

                LogFim(metodo, servicos);

                return new ObterServicosDisponiveisResponse { Servicos = servicos };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
