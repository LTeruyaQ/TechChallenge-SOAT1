using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.Jobs;

public class VerificarOrcamentoExpiradoJob(IRepositorio<OrdemServico> ordemServicoRepositorio, IInsumoOSServico insumoOSServico, IUnidadeDeTrabalho uot)
{
    private readonly IRepositorio<OrdemServico> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly IInsumoOSServico _insumoOSServico = insumoOSServico;
    private readonly IUnidadeDeTrabalho _uot = uot;

    public async Task ExecutarAsync()
    {
        var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();
        var ordensServico = await _ordemServicoRepositorio.ObterPorFiltroAsync(especificacao);

        if (!ordensServico.Any())
        {
            return;
        }

        ordensServico.ToList().ForEach(o =>
        {
            o.Status = StatusOrdemServico.OrcamentoExpirado;
        });

        await _insumoOSServico.DevolverInsumosAoEstoqueAsync(ordensServico.SelectMany(os => os.InsumosOS));

        await _ordemServicoRepositorio.EditarVariosAsync(ordensServico);

        await _uot.Commit();
    }
}