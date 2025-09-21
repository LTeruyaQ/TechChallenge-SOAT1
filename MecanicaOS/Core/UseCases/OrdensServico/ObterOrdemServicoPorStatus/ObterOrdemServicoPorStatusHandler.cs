using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus
{
    public class ObterOrdemServicoPorStatusHandler : UseCasesHandlerAbstrato<ObterOrdemServicoPorStatusHandler>, IObterOrdemServicoPorStatusHandler
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;

        public ObterOrdemServicoPorStatusHandler(
            IOrdemServicoGateway ordemServicoGateway,
            ILogServico<ObterOrdemServicoPorStatusHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
        }

        public async Task<ObterOrdemServicoPorStatusResponse> Handle(StatusOrdemServico status)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, status);

                var ordensServico = await _ordemServicoGateway.ObterOrdemServicoPorStatusAsync(status);

                LogFim(metodo, ordensServico);

                return new ObterOrdemServicoPorStatusResponse { OrdensServico = ordensServico };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
