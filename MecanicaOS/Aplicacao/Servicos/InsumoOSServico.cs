using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Requests.OrdermServico;
using Aplicacao.DTOs.Requests.OrdermServico.InsumoOrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class InsumoOSServico : ServicoAbstrato<InsumoOSServico, InsumoOS>, IInsumoOSServico
{
    private readonly IOrdemServicoServico _ordemServicoRepositorio;
    private readonly IEstoqueServico _estoqueRepositorio;

    private readonly VerificarEstoqueJob _verificarEstoqueJob;

    public InsumoOSServico(
        IOrdemServicoServico ordemServicoRepositorio,
        IEstoqueServico estoqueRepositorio,
        VerificarEstoqueJob verificarEstoqueJob,
        IRepositorio<InsumoOS> repositorio,
        ILogServico<InsumoOSServico> logServico,
        IUnidadeDeTrabalho uot, IMapper mapper) :
        base(repositorio, logServico, uot, mapper)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _estoqueRepositorio = estoqueRepositorio;
        _verificarEstoqueJob = verificarEstoqueJob;
    }

    public async Task<List<InsumoOSResponse>> CadastrarInsumosAsync(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request)
    {
        var metodo = nameof(CadastrarInsumosAsync);

        try
        {
            LogInicio(metodo);

            OrdemServico os = await ObterOrdemServicoAsync(ordemServicoId);

            await ValidarEAtualizarEstoqueAsync(request);

            List<InsumoOS> insumosOS = [.. request.Select(r =>
            {
                var insumo = _mapper.Map<InsumoOS>(r);
                insumo.OrdemServicoId = ordemServicoId;
                return insumo;
            })];

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
       return _mapper.Map<OrdemServico>(await _ordemServicoRepositorio.ObterPorIdAsync(ordemServicoId))
                ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");
    }

    private async Task ValidarEAtualizarEstoqueAsync(List<CadastrarInsumoOSRequest> insumos)
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

            await AtualizarEstoqueAsync(estoque, insumo.Quantidade);
        }

        if (insumosIndisponiveis.Count != 0)
        {
            _ = _verificarEstoqueJob.ExecutarAsync();

            throw new InsumosIndisponiveisException("Insumos insuficientes no estoque para atender ao serviço solicitado.");
        }
    }

    private async Task<Estoque> ObterEstoqueOuLancarErroAsync(Guid estoqueId)
    {
        return _mapper.Map<Estoque>(await _estoqueRepositorio.ObterPorIdAsync(estoqueId))
            ?? throw new DadosNaoEncontradosException("Insumo não encontrado no estoque.");
    }

    private static bool TemEstoqueSuficiente(Estoque estoque, int quantidadeSolicitada)
    {
        return estoque.QuantidadeDisponivel >= quantidadeSolicitada;
    }

    private async Task AtualizarEstoqueAsync(Estoque estoque, int quantidadeUtilizada)
    {
        estoque.QuantidadeDisponivel -= quantidadeUtilizada;

        await _estoqueRepositorio.AtualizarAsync(
            estoque.Id, 
            _mapper.Map<AtualizarEstoqueRequest>(estoque));
    }

    private async Task AtualizarStatusOrdemServicoAsync(OrdemServico ordemServico)
    {
        ordemServico.Status = StatusOrdemServico.EmDiagnostico;

        await _ordemServicoRepositorio.AtualizarAsync(
            ordemServico.Id,
            _mapper.Map<AtualizarOrdemServicoRequest>(ordemServico));
    }
}