using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus
{
    public class ObterOrdemServicoPorStatusHandler : UseCasesAbstrato<ObterOrdemServicoPorStatusHandler, OrdemServico>
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

        public async Task<ObterOrdemServicoPorStatusResponse> Handle(ObterOrdemServicoPorStatusUseCase useCase)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, useCase.Status);

                var ordensServico = await _ordemServicoGateway.ObterOrdemServicoPorStatusAsync(useCase.Status);

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
