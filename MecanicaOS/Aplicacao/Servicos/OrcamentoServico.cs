using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class OrcamentoServico: IOrcamentoServico
{
    public decimal GerarOrcamento(OrdemServico ordemServico)
    {
        decimal precoServico = ordemServico.Servico!.Valor;
        decimal precoInsumos = ordemServico.InsumosOS.Sum(i =>
            i.Quantidade * i.Estoque.Preco);

        return precoServico + precoInsumos;
    }
}