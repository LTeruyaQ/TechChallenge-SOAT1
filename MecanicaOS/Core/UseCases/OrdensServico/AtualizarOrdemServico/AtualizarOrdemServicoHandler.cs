using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.AtualizarOrdemServico
{
    public class AtualizarOrdemServicoHandler : UseCasesAbstrato<AtualizarOrdemServicoHandler, OrdemServico>
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;
        private readonly IEventosGateway _eventosGateway;

        public AtualizarOrdemServicoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            IEventosGateway eventosGateway,
            ILogServico<AtualizarOrdemServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
            _eventosGateway = eventosGateway ?? throw new ArgumentNullException(nameof(eventosGateway));
        }

        public async Task<AtualizarOrdemServicoResponse> Handle(AtualizarOrdemServicoCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { command.Id, command.Request });

                var ordemServico = await _ordemServicoGateway.ObterPorIdAsync(command.Id)
                    ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

                var statusAnterior = ordemServico.Status;

                // Atualizar campos
                if (!string.IsNullOrEmpty(command.Request.Descricao))
                    ordemServico.Descricao = command.Request.Descricao;

                if (command.Request.Status.HasValue)
                    ordemServico.Status = command.Request.Status.Value;

                ordemServico.DataAtualizacao = DateTime.UtcNow;

                await _ordemServicoGateway.EditarAsync(ordemServico);

                // Publicar evento se mudou para diagnóstico (simplificado)
                if (statusAnterior != StatusOrdemServico.EmDiagnostico && 
                    ordemServico.Status == StatusOrdemServico.EmDiagnostico)
                {
                    // Evento seria publicado aqui
                }

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar ordem de serviço");

                LogFim(metodo, ordemServico);

                return new AtualizarOrdemServicoResponse { OrdemServico = ordemServico };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
