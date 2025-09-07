using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Aplicacao.Servicos.Abstrato;
using Aplicacao.UseCases.Estoque.AtualizarEstoque;
using Aplicacao.UseCases.Estoque.ObterEstoque;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Insumo;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class InsumoOSServico(
    IOrdemServicoServico oSServico,
    VerificarEstoqueJob verificarEstoqueJob,
    IRepositorio<InsumoOS> repositorio,
    ILogServico<InsumoOSServico> logServico,
    IUnidadeDeTrabalho udt,
    IMapper mapper,
    IUsuarioLogadoServico usuarioLogadoServico,
    IObterEstoquePorIdUseCase obterEstoquePorIdUse,
    IAtualizarEstoqueUseCase atualizarEstoqueUseCase) : ServicoAbstrato<InsumoOSServico, InsumoOS>(repositorio, logServico, udt, mapper, usuarioLogadoServico), IInsumoOSServico
{
    private readonly IOrdemServicoServico _oSServico = oSServico;

    private readonly IObterEstoquePorIdUseCase obterEstoquePorIdUse = obterEstoquePorIdUse;

    private readonly IAtualizarEstoqueUseCase atualizarEstoqueUseCase = atualizarEstoqueUseCase;

    private readonly VerificarEstoqueJob _verificarEstoqueJob = verificarEstoqueJob;

    public async Task<List<InsumoOSResponse>> CadastrarInsumosAsync(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request)
    {
        var metodo = nameof(CadastrarInsumosAsync);

        try
        {
            LogInicio(metodo, request);

            var os = await ObterOrdemServicoAsync(ordemServicoId);

            var nvsInsumos = await RemoverInsumosJaCadastradosAsync(ordemServicoId, request);

            List<InsumoOS> insumosOS = [.. nvsInsumos.Select(r =>
            {
                var insumo = _mapper.Map<InsumoOS>(r);
                insumo.OrdemServicoId = ordemServicoId;
                return insumo;
            })];

            await ValidarEAtualizarEstoqueAsync(insumosOS);

            var entidades = await _repositorio.CadastrarVariosAsync(insumosOS);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao adicionar os insumos na ordem de serviço");

            await AtualizarStatusOrdemServicoAsync(os);

            var response = _mapper.Map<List<InsumoOSResponse>>(entidades);

            LogFim(metodo, response);

            return response;
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

            int qtdDisponivel = estoque.QuantidadeDisponivel - insumo.Quantidade;

            insumo.Estoque.Atualizar(null, null, null, qtdDisponivel, null);

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
        var insumosOS = await _repositorio.ListarAsync(especificacao);

        var estoqueIdsJaCadastrados = insumosOS.Select(x => x.EstoqueId).ToHashSet();

        insumos.RemoveAll(i => estoqueIdsJaCadastrados.Contains(i.EstoqueId));

        if (!insumos.Any())
            throw new DadosJaCadastradosException("Os insumos informados já estão cadastrados na Ordem de Serviço");

        return insumos;
    }

    private async Task<Estoque> ObterEstoqueOuLancarErroAsync(Guid estoqueId)
    {
        return await obterEstoquePorIdUse.ExecutarAsync(estoqueId);
    }

    private static bool TemEstoqueSuficiente(Estoque estoque, int quantidadeSolicitada)
    {
        return estoque.QuantidadeDisponivel >= quantidadeSolicitada;
    }

    private async Task AtualizarEstoqueAsync(Estoque estoque)
    {
        //TODO: adaptar para o clean
        await atualizarEstoqueUseCase.ExecutarAsync(estoque);
    }

    private async Task AtualizarStatusOrdemServicoAsync(OrdemServico ordemServico)
    {
        ordemServico.Status = StatusOrdemServico.EmDiagnostico;

        await _oSServico.AtualizarAsync(
            ordemServico.Id,
            _mapper.Map<AtualizarOrdemServicoRequest>(ordemServico));
    }

    public async Task DevolverInsumosAoEstoqueAsync(IEnumerable<InsumoOS> insumosOS)
    {
        var metodo = nameof(DevolverInsumosAoEstoqueAsync);

        try
        {
            LogInicio(metodo, insumosOS);

            foreach (var insumo in insumosOS)
            {
                //TODO: ajustar para o clean

                int qtdDisponivel = insumo.Estoque.QuantidadeDisponivel + insumo.Quantidade;

                insumo.Estoque.Atualizar(null, null, null, qtdDisponivel, null);

                await atualizarEstoqueUseCase.ExecutarAsync(insumo.Estoque);
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