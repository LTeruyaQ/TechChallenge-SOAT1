using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.AceitarOrcamento
{
    public class AceitarOrcamentoHandler : UseCasesAbstrato<AceitarOrcamentoHandler>
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;

        public AceitarOrcamentoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            ILogServico<AceitarOrcamentoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
        }

        public async Task<AceitarOrcamentoResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var ordemServico = await _ordemServicoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

                // Validar se está em status adequado para aceitar orçamento
                if (ordemServico.Status != StatusOrdemServico.AguardandoAprovação)
                    throw new DadosInvalidosException("Ordem de serviço não está aguardando aprovação do orçamento");

                // Verificar se orçamento não expirou (exemplo: 7 dias)
                if (ordemServico.DataEnvioOrcamento.HasValue &&
                    ordemServico.DataEnvioOrcamento.Value.AddDays(7) < DateTime.UtcNow)
                    throw new DadosInvalidosException("Orçamento expirado");

                ordemServico.Status = StatusOrdemServico.EmExecucao;
                ordemServico.DataAtualizacao = DateTime.UtcNow;

                await _ordemServicoGateway.EditarAsync(ordemServico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao aceitar orçamento");

                LogFim(metodo, ordemServico);

                return new AceitarOrcamentoResponse { Sucesso = true };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
