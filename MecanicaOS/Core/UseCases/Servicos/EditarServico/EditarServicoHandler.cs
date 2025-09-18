using Core.DTOs.UseCases.Servico;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.EditarServico
{
    public class EditarServicoHandler : UseCasesAbstrato<EditarServicoHandler>, IEditarServicoHandler
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

        public async Task<EditarServicoResponse> Handle(Guid id, EditarServicoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { id, request });

                var servico = await _servicoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Serviço não encontrado");

                servico.Nome = request.Nome;
                servico.Descricao = request.Descricao;
                servico.Valor = request.Valor.Value;
                servico.Disponivel = request.Disponivel.Value;
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
