using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Especificacoes.Insumo;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoCanceladaHandler(IRepositorio<InsumoOS> ordemServicoRepositorio, IInsumoOSServico insumoOSServico, ILogServico<OrdemServicoCanceladaHandler> logServico) : INotificationHandler<OrdemServicoCanceladaEvent>
{
    private readonly IRepositorio<InsumoOS> _insumoOSRepositorio = ordemServicoRepositorio;
    private readonly IInsumoOSServico _insumoOSServico = insumoOSServico;
    private readonly ILogServico<OrdemServicoCanceladaHandler> _logServico = logServico;

    public async Task Handle(OrdemServicoCanceladaEvent notification, CancellationToken cancellationToken)
    {
        var metodo = nameof(Handle);

        try
        {
            _logServico.LogInicio(metodo, notification.OrdemServicoId);

            var especificacao = new ObterInsumosOSPorOSEspecificacao(notification.OrdemServicoId);
            var insumosOS = await _insumoOSRepositorio.ListarAsync(especificacao);

            if (!insumosOS.Any()) return;

            await _insumoOSServico.DevolverInsumosAoEstoqueAsync(insumosOS);

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }
}