using Core.Entidades;
using Core.Interfaces.UseCases;

namespace Core.UseCases;

public class OrcamentoUseCases : IOrcamentoUseCases
{
    public decimal GerarOrcamentoUseCase(OrdemServico ordemServico)
    {
        decimal precoServico = ordemServico.Servico!.Valor;
        decimal precoInsumos = ordemServico.InsumosOS.Sum(i =>
            i.Quantidade * i.Estoque.Preco);

        return precoServico + precoInsumos;
    }
}