using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.ObterServicoPorNome
{
    public class ObterServicoPorNomeHandler : UseCasesAbstrato<ObterServicoPorNomeHandler>
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

                var servico = await _servicoGateway.ObterServicosDisponiveisPorNomeAsync(nome);

                LogFim(metodo, servico);

                return new ObterServicoPorNomeResponse { Servico = servico };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
