using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Interfaces.Repositorios;
using MediatR;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoCanceladaHandler(IRepositorio<InsumoOS> ordemServicoRepositorio, IRepositorio<Estoque> estoqueRepositorio, IUnidadeDeTrabalho uot) : INotificationHandler<OrdemServicoCanceladaEvent>
{
    private readonly IRepositorio<InsumoOS> _insumoOSRepositorio = ordemServicoRepositorio;
    private readonly IRepositorio<Estoque> _estoqueRepositorio = estoqueRepositorio;
    private readonly IUnidadeDeTrabalho _uot = uot;

    public async Task Handle(OrdemServicoCanceladaEvent notification, CancellationToken cancellationToken)
    {
        var especificacao = new ObterInsumosOSPorOSEspecificacao(notification.OrdemServicoId);
        var insumosOS = await _insumoOSRepositorio.ObterPorFiltroAsync(especificacao);

        if (!insumosOS.Any()) return;

        foreach (var insumo in insumosOS)
        {
            insumo.Estoque.QuantidadeDisponivel += insumo.Quantidade;
        }

        await _estoqueRepositorio.EditarVariosAsync(insumosOS.Select(o => o.Estoque));
        await _uot.Commit();
    }
}