using Core.DTOs.UseCases.Estoque;
using Core.DTOs.UseCases.OrdemServico;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases;

public class InsumoOSUseCases(
    IOrdemServicoUseCases oSUseCases,
    IEstoqueUseCases estoqueUseCases,
    IInsumosGateway insumosGateway,
    ILogServico<InsumoOSUseCases> logServico,
    IUnidadeDeTrabalho udt,
    IUsuarioLogadoServico usuarioLogadoServico,
    IVerificarEstoqueJobGateway verificarEstoqueJobGateway) : UseCasesAbstrato<InsumoOSUseCases, InsumoOS>(logServico, udt, usuarioLogadoServico), IInsumoOSUseCases
{
    private readonly IInsumosGateway _insumosGateway = insumosGateway;
    private readonly IOrdemServicoUseCases _oSUseCases = oSUseCases;
    private readonly IEstoqueUseCases _estoqueUseCases = estoqueUseCases;
    private readonly IVerificarEstoqueJobGateway _verificarEstoqueJobGateway = verificarEstoqueJobGateway;

    public async Task<IEnumerable<InsumoOS>> CadastrarInsumosUseCaseAsync(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request)
    {
        var metodo = nameof(CadastrarInsumosUseCaseAsync);

        try
        {
            LogInicio(metodo, request);

            var os = await ObterOrdemServicoAsync(ordemServicoId);

            var nvsInsumos = await RemoverInsumosJaCadastradosAsync(ordemServicoId, request);

            List<InsumoOS> insumosOS = [.. nvsInsumos.Select(r =>
            {
                var insumo = new InsumoOS(){
                    EstoqueId = r.EstoqueId,
                    Quantidade = r.Quantidade
                };
                insumo.OrdemServicoId = ordemServicoId;
                return insumo;
            })];

            await ValidarEAtualizarEstoqueAsync(insumosOS);

            var entidades = await _insumosGateway.CadastrarVariosAsync(insumosOS);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao adicionar os insumos na ordem de serviço");

            await AtualizarStatusOrdemServicoAsync(os);

            LogFim(metodo, entidades);

            return entidades;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    private async Task<OrdemServico> ObterOrdemServicoAsync(Guid ordemServicoId)
    {
        return await _oSUseCases.ObterPorIdUseCaseAsync(ordemServicoId)
                 ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");
    }

    private async Task ValidarEAtualizarEstoqueAsync(List<InsumoOS> insumos)
    {
        var insumosIndisponiveis = new List<Estoque>();

        foreach (var insumo in insumos)
        {
            var estoque = await ObterEstoqueOuLancarErroAsync(insumo.EstoqueId);

            if (!TemEstoqueSuficiente(estoque, insumo.Quantidade))
            {
                insumosIndisponiveis.Add(estoque);
                continue;
            }

            estoque.QuantidadeDisponivel -= insumo.Quantidade;

            await AtualizarEstoqueAsync(estoque);
        }

        if (insumosIndisponiveis.Count != 0)
        {
            _ = _verificarEstoqueJobGateway.VerificarEstoqueAsync();

            throw new InsumosIndisponiveisException("Insumos insuficientes no estoque para atender ao serviço solicitado.");
        }
    }

    private async Task<IEnumerable<CadastrarInsumoOSUseCaseDto>> RemoverInsumosJaCadastradosAsync(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> insumos)
    {
        var insumosOS = await _insumosGateway.ObterInsumosOSPorOSAsync(ordemServicoId);

        var estoqueIdsJaCadastrados = insumosOS.Select(x => x.EstoqueId).ToHashSet();

        insumos.RemoveAll(i => estoqueIdsJaCadastrados.Contains(i.EstoqueId));

        if (!insumos.Any())
            throw new DadosJaCadastradosException("Os insumos informados já estão cadastrados na Ordem de Serviço");

        return insumos;
    }

    private async Task<Estoque> ObterEstoqueOuLancarErroAsync(Guid estoqueId)
    {
        return await _estoqueUseCases.ObterPorIdUseCaseAsync(estoqueId)
            ?? throw new DadosNaoEncontradosException("Insumo não encontrado no estoque.");
    }

    private static bool TemEstoqueSuficiente(Estoque estoque, int quantidadeSolicitada)
    {
        return estoque.QuantidadeDisponivel >= quantidadeSolicitada;
    }

    private async Task AtualizarEstoqueAsync(Estoque estoque)
    {
        AtualizarEstoqueUseCaseDto dto = new()
        {
            QuantidadeDisponivel = estoque.QuantidadeDisponivel,
        };
        await _estoqueUseCases.AtualizarUseCaseAsync(
            estoque.Id,
            dto);
    }

    private async Task AtualizarStatusOrdemServicoAsync(OrdemServico ordemServico)
    {
        ordemServico.Status = StatusOrdemServico.EmDiagnostico;
        AtualizarOrdemServicoUseCaseDto dto = new()
        {
            Status = ordemServico.Status,
        };
        await _oSUseCases.AtualizarUseCaseAsync(
            ordemServico.Id,
            dto);
    }

    public async Task DevolverInsumosAoEstoqueUseCaseAsync(IEnumerable<InsumoOS> insumosOS)
    {
        var metodo = nameof(DevolverInsumosAoEstoqueUseCaseAsync);

        try
        {
            LogInicio(metodo, insumosOS);

            foreach (var insumo in insumosOS)
            {
                insumo.Estoque.QuantidadeDisponivel += insumo.Quantidade;
                AtualizarEstoqueUseCaseDto dto = new()
                {
                    QuantidadeDisponivel = insumo.Estoque.QuantidadeDisponivel,
                };
                await _estoqueUseCases.AtualizarUseCaseAsync(
                    insumo.EstoqueId,
                    dto);
            }

            LogFim(metodo);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }
}