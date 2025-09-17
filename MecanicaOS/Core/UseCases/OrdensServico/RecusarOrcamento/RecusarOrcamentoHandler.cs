using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.RecusarOrcamento
{
    public class RecusarOrcamentoHandler : UseCasesAbstrato<RecusarOrcamentoHandler>
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;

        public RecusarOrcamentoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            ILogServico<RecusarOrcamentoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
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
