using Core.DTOs.Repositories.OrdemServicos;
using Core.Entidades;
using Core.Especificacoes.Insumo;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using MediatR;

namespace Infraestrutura.Notificacoes.OS;

public class OrdemServicoCanceladaHandler(IRepositorio<InsumoOSRepositoryDto> ordemServicoRepositorio, IInsumoOSUseCases insumoOSUseCases, ILogServico<OrdemServicoCanceladaHandler> logServico) : INotificationHandler<OrdemServicoCanceladaEvent>
{
    private readonly IRepositorio<InsumoOSRepositoryDto> _insumoOSRepositorio = ordemServicoRepositorio;
    private readonly IInsumoOSUseCases _insumoOSUseCases = insumoOSUseCases;
    private readonly ILogServico<OrdemServicoCanceladaHandler> _logServico = logServico;

    public async Task Handle(OrdemServicoCanceladaEvent notification, CancellationToken cancellationToken)
    {
        var metodo = nameof(Handle);

        try
        {
            _logServico.LogInicio(metodo, notification.OrdemServicoId);

            var especificacao = new ObterInsumosOSPorOSEspecificacao(notification.OrdemServicoId);
            var insumosOSDto = await _insumoOSRepositorio.ListarSemRastreamentoAsync(especificacao);

            if (!insumosOSDto.Any()) return;

            var insumosOS = insumosOSDto.Select(dto => new InsumoOS
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                OrdemServicoId = dto.OrdemServicoId,
                EstoqueId = dto.EstoqueId,
                Quantidade = dto.Quantidade
            });

            await _insumoOSUseCases.DevolverInsumosAoEstoqueUseCaseAsync(insumosOS);

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }
}