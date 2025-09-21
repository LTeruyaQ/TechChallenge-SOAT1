using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.DeletarServico
{
    public class DeletarServicoHandler : UseCasesHandlerAbstrato<DeletarServicoHandler>, IDeletarServicoHandler
    {
        private readonly IServicoGateway _servicoGateway;

        public DeletarServicoHandler(
            IServicoGateway servicoGateway,
            ILogServico<DeletarServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<DeletarServicoResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var servico = await _servicoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Serviço não encontrado");

                await _servicoGateway.DeletarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao deletar serviço");

                LogFim(metodo, true);

                return new DeletarServicoResponse { Sucesso = true };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
