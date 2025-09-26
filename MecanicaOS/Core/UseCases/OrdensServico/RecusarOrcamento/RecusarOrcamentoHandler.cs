using Core.DTOs.UseCases.Eventos;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.RecusarOrcamento
{
    public class RecusarOrcamentoHandler : UseCasesHandlerAbstrato<RecusarOrcamentoHandler>, IRecusarOrcamentoHandler
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;
        private readonly IEventosGateway _eventosGateway;

        public RecusarOrcamentoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            IEventosGateway eventosGateway,
            ILogServicoGateway<RecusarOrcamentoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
            _eventosGateway = eventosGateway ?? throw new ArgumentNullException(nameof(eventosGateway));
        }

        public async Task<RecusarOrcamentoResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var ordemServico = await _ordemServicoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

                // Validar se está em status adequado para recusar orçamento
                if (ordemServico.Status != StatusOrdemServico.AguardandoAprovação)
                    throw new DadosInvalidosException("Ordem de serviço não está aguardando aprovação do orçamento");

                ordemServico.Status = StatusOrdemServico.Cancelada;
                ordemServico.DataAtualizacao = DateTime.UtcNow;

                await _ordemServicoGateway.EditarAsync(ordemServico);
                
                // Publicar evento de ordem de serviço cancelada
                await _eventosGateway.Publicar(new OrdemServicoCanceladaEventDTO(ordemServico.Id));

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao recusar orçamento");

                LogFim(metodo, ordemServico);

                return new RecusarOrcamentoResponse { Sucesso = true };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
