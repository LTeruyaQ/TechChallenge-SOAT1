using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.ObterTodosServicos
{
    public class ObterTodosServicosHandler : UseCasesAbstrato<ObterTodosServicosHandler, Servico>
    {
        private readonly IServicoGateway _servicoGateway;

        public ObterTodosServicosHandler(
            IServicoGateway servicoGateway,
            ILogServico<ObterTodosServicosHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<ObterTodosServicosResponse> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var servicos = await _servicoGateway.ObterTodosAsync();

                LogFim(metodo, servicos);

                return new ObterTodosServicosResponse { Servicos = servicos };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
