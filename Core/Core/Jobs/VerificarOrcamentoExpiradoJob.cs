using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;

namespace Aplicacao.Jobs;

public class VerificarOrcamentoExpiradoJob(IOrdemServicoGateway ordemServicoGateway, IInsumoOSUseCases insumoOSServico, IUnidadeDeTrabalho udt, ILogServico<VerificarOrcamentoExpiradoJob> logServico)
{
    private readonly IOrdemServicoGateway _ordemServicoGateway = ordemServicoGateway;
    private readonly IInsumoOSUseCases _insumoOSUseCases = insumoOSServico;
    private readonly IUnidadeDeTrabalho _uot = udt;
    private readonly ILogServico<VerificarOrcamentoExpiradoJob> _logServico = logServico;

    public async Task ExecutarAsync()
    {
        var metodo = nameof(ExecutarAsync);

        try
        {
            _logServico.LogInicio(metodo);

            //var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();
            //var ordensServico = await _ordemServicoGateway.ListarAsync(especificacao);
            var ordensServico = await _ordemServicoGateway.ListarOSOrcamentoExpiradoAsync();

            if (!ordensServico.Any())
            {
                return;
            }

            ordensServico.ToList().ForEach(o =>
            {
                o.Status = StatusOrdemServico.OrcamentoExpirado;
            });

            await _insumoOSUseCases.DevolverInsumosAoEstoqueUseCaseAsync(ordensServico.SelectMany(os => os.InsumosOS));

            await _ordemServicoGateway.EditarVariosAsync(ordensServico);

            await _uot.Commit();

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }
}