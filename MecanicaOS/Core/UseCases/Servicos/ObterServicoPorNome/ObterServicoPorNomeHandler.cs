using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.ObterServicoPorNome
{
    public class ObterServicoPorNomeHandler : UseCasesHandlerAbstrato<ObterServicoPorNomeHandler>, IObterServicoPorNomeHandler
    {
        private readonly IServicoGateway _servicoGateway;

        public ObterServicoPorNomeHandler(
            IServicoGateway servicoGateway,
            ILogServico<ObterServicoPorNomeHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<ObterServicoPorNomeResponse> Handle(string nome)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, nome);

                if (string.IsNullOrWhiteSpace(nome))
                {
                    LogFim(metodo, null);
                    return new ObterServicoPorNomeResponse { Servico = null };
                }

                var servico = await _servicoGateway.ObterServicosDisponiveisPorNomeAsync(nome);

                var response = new ObterServicoPorNomeResponse { Servico = servico };
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
