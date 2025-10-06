using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.AtualizarOrdemServico
{
    public class AtualizarOrdemServicoHandler : UseCasesHandlerAbstrato<AtualizarOrdemServicoHandler>, IAtualizarOrdemServicoHandler
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;
        private readonly IEventosGateway _eventosGateway;

        public AtualizarOrdemServicoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            IEventosGateway eventosGateway,
            ILogGateway<AtualizarOrdemServicoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
            _eventosGateway = eventosGateway;
        }

        public async Task<OrdemServico> Handle(Guid id, AtualizarOrdemServicoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { id, request });

                var ordemServico = await _ordemServicoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

                var statusAnterior = ordemServico.Status;

                if (!string.IsNullOrEmpty(request.Descricao))
                    ordemServico.Descricao = request.Descricao;

                if (request.Status.HasValue)
                    ordemServico.Status = request.Status.Value;

                if (request.Orcamento.HasValue)
                    ordemServico.Orcamento = request.Orcamento.Value;

                if (request.DataEnvioOrcamento.HasValue)
                    ordemServico.DataEnvioOrcamento = request.DataEnvioOrcamento.Value;

                if (request.ClienteId.HasValue)
                    ordemServico.ClienteId = request.ClienteId.Value;

                if (request.VeiculoId.HasValue)
                    ordemServico.VeiculoId = request.VeiculoId.Value;

                if (request.ServicoId.HasValue)
                    ordemServico.ServicoId = request.ServicoId.Value;

                ordemServico.DataAtualizacao = DateTime.UtcNow;

                await _ordemServicoGateway.EditarAsync(ordemServico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar ordem de serviço");

                if (statusAnterior != ordemServico.Status)
                {
                    await _eventosGateway.Publicar(ordemServico);
                }

                LogFim(metodo, ordemServico);

                return ordemServico;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
