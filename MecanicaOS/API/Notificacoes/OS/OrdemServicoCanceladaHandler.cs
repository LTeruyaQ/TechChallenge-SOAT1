using API;
using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Especificacoes.Insumo;
using Core.Interfaces.Controllers;
using Core.Interfaces.Repositorios;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using Infraestrutura.Dados;
using MediatR;

namespace Infraestrutura.Notificacoes.OS;

public class OrdemServicoCanceladaHandler : INotificationHandler<OrdemServicoCanceladaEvent>
{
    private readonly IRepositorio<InsumoOSEntityDto> _insumoOSRepositorio;
    private readonly IOrdemServicoController _ordemServicoController;
    private readonly ILogServico<OrdemServicoCanceladaHandler> _logServico;

    public OrdemServicoCanceladaHandler(ICompositionRoot compositionRoot)
    {
        _insumoOSRepositorio = compositionRoot.CriarRepositorio<InsumoOSEntityDto>();
        _ordemServicoController = compositionRoot.CriarOrdemServicoController();
        _logServico = compositionRoot.CriarLogService<OrdemServicoCanceladaHandler>();
    }

    public async Task Handle(OrdemServicoCanceladaEvent notification, CancellationToken cancellationToken)
    {
        var metodo = nameof(Handle);

        try
        {
            _logServico.LogInicio(metodo, notification.OrdemServicoId);

            await _ordemServicoController.CalcularOrcamentoAsync(notification.OrdemServicoId);

            var especificacao = new ObterInsumosOSPorOSEspecificacao(notification.OrdemServicoId);
            var insumosOSDto = await _insumoOSRepositorio.ListarSemRastreamentoAsync(especificacao);

            if (!insumosOSDto.Any()) return;

            // Usar o Controller para devolver os insumos ao estoque
            //var insumosOS = insumosOSDto.Select(dto => new InsumoOS
            //{
            //    Id = dto.Id,
            //    Ativo = dto.Ativo,
            //    DataCadastro = dto.DataCadastro,
            //    DataAtualizacao = dto.DataAtualizacao,
            //    OrdemServicoId = dto.OrdemServicoId,
            //    EstoqueId = dto.EstoqueId,
            //    Quantidade = dto.Quantidade
            //});

            //await _insumoOSUController.DevolverInsumosAoEstoqueUseCaseAsync(insumosOS);

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }
}