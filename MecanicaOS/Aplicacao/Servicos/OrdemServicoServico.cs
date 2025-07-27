using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Notificacoes.OS;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Cliente;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;

namespace Aplicacao.Servicos;

public class OrdemServicoServico : ServicoAbstrato<OrdemServicoServico, OrdemServico>, IOrdemServicoServico
{
    private readonly IMediator _mediator;
    private readonly IRepositorio<Cliente> _clienteRepositorio;
    private readonly IServicoServico _servicoServico;

    public OrdemServicoServico(
        IRepositorio<OrdemServico> repositorio,
        ILogServico<OrdemServicoServico> logServico,
        IUnidadeDeTrabalho uot,
        IMapper mapper,
        IMediator mediator,
        IRepositorio<Cliente> clienteRepositorio,
        IServicoServico servicoServico, 
        IUsuarioLogadoServico usuarioLogadoServico) :
        base(repositorio, logServico, uot, mapper, usuarioLogadoServico)
    {
        _mediator = mediator;
        _clienteRepositorio = clienteRepositorio;
        _servicoServico = servicoServico;
    }

    public async Task<OrdemServicoResponse> AtualizarAsync(Guid id, AtualizarOrdemServicoRequest request)
    {
        var metodo = nameof(AtualizarAsync);

        try
        {
            LogInicio(metodo, new { id, request });

            OrdemServico ordemServico = await _repositorio.ObterPorIdAsync(id) ??
                throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

            ordemServico.Atualizar(
                request.ClienteId,
                request.VeiculoId,
                request.ServicoId,
                request.Descricao,
                request.Status);

            await _repositorio.EditarAsync(ordemServico);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar a ordem de serviço");

            LogFim(metodo, ordemServico);

            //TODO: refatorar isso
            if (ordemServico.Status == StatusOrdemServico.EmDiagnostico)
            {
                await _mediator.Publish(new OrdemServicoEmOrcamentoEvent(ordemServico.Id));
            }
            else if (ordemServico.Status == StatusOrdemServico.Cancelada)
            {
                await _mediator.Publish(new OrdemServicoCanceladaEvent(ordemServico.Id));
            }
            else if (ordemServico.Status == StatusOrdemServico.Finalizada)
            {
                await _mediator.Publish(new OrdemServicoFinalizadaEvent(ordemServico.Id));
            }

            return _mapper.Map<OrdemServicoResponse>(ordemServico);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<OrdemServicoResponse> CadastrarAsync(CadastrarOrdemServicoRequest request)
    {
        var metodo = nameof(CadastrarAsync);

        try
        {
            LogInicio(metodo, request);

            await VerificarEntidadesRelacionadasAsync(request);

            var ordemServico = _mapper.Map<OrdemServico>(request);

            var entidade = await _repositorio.CadastrarAsync(ordemServico);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao cadastrar a ordem de serviço");

            LogFim(metodo, entidade);

            return _mapper.Map<OrdemServicoResponse>(entidade);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    private async Task VerificarEntidadesRelacionadasAsync(CadastrarOrdemServicoRequest request)
    {
        var especificacao = new ObterClienteComVeiculoPorIdEspecificacao(request.ClienteId);
        var cliente = await _clienteRepositorio.ObterUmAsync(especificacao)
            ?? throw new DadosNaoEncontradosException("O cliente não foi encontrado");

        var veiculo = cliente.Veiculos.FirstOrDefault(v => v.Id == request.VeiculoId);
        _ = veiculo ?? throw new DadosNaoEncontradosException("O veículo não foi encontrado");

        _ = await _servicoServico.ObterServicoPorIdAsync(request.ServicoId);
    }

    public async Task<OrdemServicoResponse?> ObterPorIdAsync(Guid id)
    {
        var metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            var especificacao = new ObterOrdemServicoPorIdComInsumosEspecificacao(id);
            var ordemServico = await _repositorio.ObterUmSemRastreamentoAsync(especificacao);

            LogFim(metodo, ordemServico);

            return _mapper.Map<OrdemServicoResponse>(ordemServico);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<IEnumerable<OrdemServicoResponse>> ObterPorStatusAsync(StatusOrdemServico status)
    {
        var metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            var filtroOSStatus = new ObterOrdemServicoPorStatusEspecificacao(status);

            var ordemServico = await _repositorio.ObterPorFiltroAsync(filtroOSStatus);

            LogFim(metodo, ordemServico);

            return _mapper.Map<IEnumerable<OrdemServicoResponse>>(ordemServico);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<IEnumerable<OrdemServicoResponse>> ObterTodosAsync()
    {
        var metodo = nameof(ObterTodosAsync);

        try
        {
            LogInicio(metodo);

            var ordensServico = await _repositorio.ObterTodosAsync();

            LogFim(metodo, ordensServico);

            return _mapper.Map<IEnumerable<OrdemServicoResponse>>(ordensServico);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task AceitarOrcamentoAsync(Guid id)
    {
        var metodo = nameof(AceitarOrcamentoAsync);

        try
        {
            LogInicio(metodo, new { id });

            await ProcessarRespostaOrcamentoAsync(id, true);

            LogFim(metodo, new { id });
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task RecusarOrcamentoAsync(Guid id)
    {
        var metodo = nameof(RecusarOrcamentoAsync);

        try
        {
            LogInicio(metodo, new { id });

            await ProcessarRespostaOrcamentoAsync(id, false);

            LogFim(metodo, new { id });
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    private async Task ProcessarRespostaOrcamentoAsync(Guid id, bool aceito)
    {
        OrdemServico ordemServico = await _repositorio.ObterPorIdAsync(id) ??
            throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

        await VerificarOrcamentoExpiradoAsync(ordemServico);

        if (aceito)
            ordemServico.Status = StatusOrdemServico.EmExecucao;
        else
        {
            ordemServico.Status = StatusOrdemServico.Cancelada;
            await _mediator.Publish(new OrdemServicoCanceladaEvent(id));
        }

        await _repositorio.EditarAsync(ordemServico);

        if (!await Commit())
            throw new PersistirDadosException($"Erro ao {(aceito ? "aceitar" : "recusar")} o orçamento da ordem de serviço");
    }

    private async Task VerificarOrcamentoExpiradoAsync(OrdemServico ordemServico)
    {
        if (ordemServico.Status == StatusOrdemServico.OrcamentoExpirado)
            throw new OrcamentoExpiradoException("Orçamento expirado");

        bool deveExpirar = ordemServico.Status == StatusOrdemServico.AguardandoAprovação
            && ordemServico.DataEnvioOrcamento.HasValue
            && ordemServico.DataEnvioOrcamento.Value.AddDays(3) <= DateTime.UtcNow;

        if (deveExpirar)
        {
            ordemServico.Status = StatusOrdemServico.OrcamentoExpirado;

            await _repositorio.EditarAsync(ordemServico);
            await Commit();

            throw new OrcamentoExpiradoException("Orçamento expirado");
        }
    }
}