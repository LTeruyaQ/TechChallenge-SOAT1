using Core.DTOs.UseCases.OrdemServico;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.AtualizarOrdemServico
{
    public class AtualizarOrdemServicoHandler : UseCasesAbstrato<AtualizarOrdemServicoHandler>, IAtualizarOrdemServicoHandler
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;

        public AtualizarOrdemServicoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            ILogServico<AtualizarOrdemServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
        }

        public async Task<AtualizarOrdemServicoResponse> Handle(Guid id, AtualizarOrdemServicoUseCaseDto request)
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

                if(request.Orcamento.HasValue)
                    ordemServico.Orcamento = request.Orcamento.Value;

                if (request.DataEnvioOrcamento.HasValue)
                    ordemServico.DataEnvioOrcamento = request.DataEnvioOrcamento.Value;

                if(request.ClienteId.HasValue)
                    ordemServico.ClienteId = request.ClienteId.Value;

                if(request.VeiculoId.HasValue)
                    ordemServico.VeiculoId = request.VeiculoId.Value;

                if(request.ServicoId.HasValue)
                    ordemServico.ServicoId = request.ServicoId.Value;

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
