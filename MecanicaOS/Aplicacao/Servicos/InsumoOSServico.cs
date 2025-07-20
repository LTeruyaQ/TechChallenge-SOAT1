using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class InsumoOSServico(
    IOrdemServicoServico oSServico,
    IEstoqueServico estoqueServico,
    VerificarEstoqueJob verificarEstoqueJob,
    IRepositorio<InsumoOS> repositorio,
    ILogServico<InsumoOSServico> logServico,
    IUnidadeDeTrabalho uot,
    IMapper mapper) : ServicoAbstrato<InsumoOSServico, InsumoOS>(repositorio, logServico, uot, mapper), IInsumoOSServico
{
    private readonly IOrdemServicoServico _oSServico = oSServico;
    private readonly IEstoqueServico _estoqueServico = estoqueServico;

    private readonly VerificarEstoqueJob _verificarEstoqueJob = verificarEstoqueJob;

    public async Task<List<InsumoOSResponse>> CadastrarInsumosAsync(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request)
    {
        var metodo = nameof(CadastrarInsumosAsync);

        try
        {
            LogInicio(metodo);

            var os = await ObterOrdemServicoAsync(ordemServicoId);

            var nvsInsumos = await RemoverInsumosJaCadastradosAsync(ordemServicoId, request);

            List<InsumoOS> insumosOS = [.. nvsInsumos.Select(r =>
            {
                var insumo = _mapper.Map<InsumoOS>(r);
                insumo.OrdemServicoId = ordemServicoId;
                return insumo;
            })];

            await ValidarEAtualizarEstoqueAsync(os.Id, insumosOS);

            var entidades = await _repositorio.CadastrarVariosAsync(insumosOS);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao adicionar os insumos na ordem de serviço");

            await AtualizarStatusOrdemServicoAsync(os);

            LogFim(metodo, entidades);

            return _mapper.Map<List<InsumoOSResponse>>(entidades);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    private async Task<OrdemServico> ObterOrdemServicoAsync(Guid ordemServicoId)
    {
        return _mapper.Map<OrdemServico>(await _oSServico.ObterPorIdAsync(ordemServicoId))
                 ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");
    }

    private async Task ValidarEAtualizarEstoqueAsync(Guid ordemServicoId, List<InsumoOS> insumos)
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
            _ = _verificarEstoqueJob.ExecutarAsync();

            throw new InsumosIndisponiveisException("Insumos insuficientes no estoque para atender ao serviço solicitado.");
        }
    }

    private async Task<List<CadastrarInsumoOSRequest>> RemoverInsumosJaCadastradosAsync(Guid ordemServicoId, List<CadastrarInsumoOSRequest> insumos)
    {
        var especificacao = new ObterInsumosOSPorOSEspecificacao(ordemServicoId);
        var insumosOS = await _repositorio.ObterPorFiltroAsync(especificacao);

        var estoqueIdsJaCadastrados = insumosOS.Select(x => x.EstoqueId).ToHashSet();

        insumos.RemoveAll(i => estoqueIdsJaCadastrados.Contains(i.EstoqueId));

        if (!insumos.Any())
            throw new DadosJaCadastradosException("Os insumos informados já estão cadastrados na Ordem de Serviço");

        return insumos;
    }

    private async Task<Estoque> ObterEstoqueOuLancarErroAsync(Guid estoqueId)
    {
        return _mapper.Map<Estoque>(await _estoqueServico.ObterPorIdAsync(estoqueId))
            ?? throw new DadosNaoEncontradosException("Insumo não encontrado no estoque.");
    }

    private static bool TemEstoqueSuficiente(Estoque estoque, int quantidadeSolicitada)
    {
        return estoque.QuantidadeDisponivel >= quantidadeSolicitada;
    }

    private async Task AtualizarEstoqueAsync(Estoque estoque)
    {
        await _estoqueServico.AtualizarAsync(
            estoque.Id,
            _mapper.Map<AtualizarEstoqueRequest>(estoque));
    }

    private async Task AtualizarStatusOrdemServicoAsync(OrdemServico ordemServico)
    {
        ordemServico.Status = StatusOrdemServico.EmDiagnostico;

        await _oSServico.AtualizarAsync(
            ordemServico.Id,
            _mapper.Map<AtualizarOrdemServicoRequest>(ordemServico));
    }
}