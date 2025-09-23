using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.ObterServico
{
    public class ObterServicoHandler : UseCasesHandlerAbstrato<ObterServicoHandler>, IObterServicoHandler
    {
        private readonly IServicoGateway _servicoGateway;

        public ObterServicoHandler(
            IServicoGateway servicoGateway,
            ILogServico<ObterServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<ObterServicoResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var servico = await _servicoGateway.ObterPorIdAsync(id);
                if (servico == null)
                    throw new DadosNaoEncontradosException("Serviço não encontrado");

                var response = new ObterServicoResponse { Servico = servico };
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
