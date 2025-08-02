using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Jobs;

public class VerificarOrcamentoExpiradoJob(IRepositorio<OrdemServico> ordemServicoRepositorio, IInsumoOSServico insumoOSServico, IUnidadeDeTrabalho udt, ILogServico<VerificarOrcamentoExpiradoJob> logServico)
{
    private readonly IRepositorio<OrdemServico> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly IInsumoOSServico _insumoOSServico = insumoOSServico;
    private readonly IUnidadeDeTrabalho _uot = udt;
    private readonly ILogServico<VerificarOrcamentoExpiradoJob> _logServico = logServico;

    public async Task ExecutarAsync()
    {
        var metodo = nameof(ExecutarAsync);

        try
        {
            _logServico.LogInicio(metodo);

            var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();
            var ordensServico = await _ordemServicoRepositorio.ListarAsync(especificacao);

            if (ordensServico.Any())
            {
                ordensServico.ToList().ForEach(o =>
                {
                    o.Status = StatusOrdemServico.OrcamentoExpirado;
                });

                await _insumoOSServico.DevolverInsumosAoEstoqueAsync(ordensServico.SelectMany(os => os.InsumosOS));
                await _ordemServicoRepositorio.EditarVariosAsync(ordensServico);
                await _uot.Commit();
            }
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);
            throw;
        }
        finally
        {
            _logServico.LogFim(metodo);
        }
    }
}