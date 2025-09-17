using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.EditarServico
{
    public class EditarServicoHandler : UseCasesAbstrato<EditarServicoHandler, Servico>
    {
        private readonly IServicoGateway _servicoGateway;

        public EditarServicoHandler(
            IServicoGateway servicoGateway,
            ILogServico<EditarServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<EditarServicoResponse> Handle(EditarServicoCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { command.Id, command.Request });

                var servico = await _servicoGateway.ObterPorIdAsync(command.Id)
                    ?? throw new DadosNaoEncontradosException("Serviço não encontrado");

                servico.Nome = command.Request.Nome;
                servico.Descricao = command.Request.Descricao;
                servico.Valor = command.Request.Valor.Value;
                servico.Disponivel = command.Request.Disponivel.Value;
                servico.DataAtualizacao = DateTime.UtcNow;

                await _servicoGateway.EditarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao editar serviço");

                LogFim(metodo, servico);

                return new EditarServicoResponse { Servico = servico };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
