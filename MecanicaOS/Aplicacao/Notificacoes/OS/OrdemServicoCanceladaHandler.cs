using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Interfaces.Repositorios;
using MediatR;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoCanceladaHandler(IRepositorio<InsumoOS> ordemServicoRepositorio, IInsumoOSServico insumoOSServico) : INotificationHandler<OrdemServicoCanceladaEvent>
{
    private readonly IRepositorio<InsumoOS> _insumoOSRepositorio = ordemServicoRepositorio;
    private readonly IInsumoOSServico _insumoOSServico = insumoOSServico;

    public async Task Handle(OrdemServicoCanceladaEvent notification, CancellationToken cancellationToken)
    {
        var especificacao = new ObterInsumosOSPorOSEspecificacao(notification.OrdemServicoId);
        var insumosOS = await _insumoOSRepositorio.ObterPorFiltroAsync(especificacao);
        
        if (!insumosOS.Any()) return;

        await _insumoOSServico.DevolverInsumosAoEstoqueAsync(insumosOS);
    }
}