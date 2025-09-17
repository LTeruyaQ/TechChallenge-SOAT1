using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.DeletarServico
{
    public class DeletarServicoHandler : UseCasesAbstrato<DeletarServicoHandler, Servico>
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

        public async Task<DeletarServicoResponse> Handle(DeletarServicoCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, command.Id);

                var servico = await _servicoGateway.ObterPorIdAsync(command.Id)
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
