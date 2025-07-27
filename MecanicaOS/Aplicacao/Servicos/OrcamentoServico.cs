using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class OrcamentoServico(ILogServico<OrcamentoServico> logServico) : IOrcamentoServico
{
    private readonly ILogServico<OrcamentoServico> _logServico = logServico;

    public decimal GerarOrcamento(OrdemServico ordemServico)
    {
        var metodo = nameof(GerarOrcamento);

        try
        {
            _logServico.LogInicio(metodo, ordemServico);

            decimal precoServico = ordemServico.Servico!.Valor;
            decimal precoInsumos = ordemServico.InsumosOS.Sum(i =>
                i.Quantidade * i.Estoque.Preco);

            _logServico.LogFim(metodo);

            return precoServico + precoInsumos;
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }
}