using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.ObterOrdemServico
{
    public class ObterOrdemServicoHandler : UseCasesAbstrato<ObterOrdemServicoHandler, OrdemServico>
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;

        public ObterOrdemServicoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            ILogServico<ObterOrdemServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
        }

        public async Task<ObterOrdemServicoResponse> Handle(ObterOrdemServicoUseCase useCase)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, useCase.Id);

                var ordemServico = await _ordemServicoGateway.ObterPorIdAsync(useCase.Id);

                LogFim(metodo, ordemServico);

                return new ObterOrdemServicoResponse { OrdemServico = ordemServico };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
