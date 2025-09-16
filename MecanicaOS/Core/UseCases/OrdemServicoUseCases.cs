using Core.DTOs.UseCases.Eventos;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases;

public class OrdemServicoUseCases : UseCasesAbstrato<OrdemServicoUseCases, OrdemServico>, IOrdemServicoUseCases
{
    private readonly IClienteGateway _clienteGateway;
    private readonly IServicoUseCases _servicoUseCase;
    private readonly IEventosGateway _eventosGateway;
    private readonly IOrdemServicoGateway _ordemServicoGateway;

    public OrdemServicoUseCases(
        ILogServico<OrdemServicoUseCases> logServico,
        IUnidadeDeTrabalho udt,
        IClienteGateway clienteGateway,
        IServicoUseCases servicoUseCase,
        IUsuarioLogadoServico usuarioLogadoServico,
        IOrdemServicoGateway ordemServicoGateway,
        IEventosGateway eventosGateway) :
        base(logServico, udt, usuarioLogadoServico)
    {
        _clienteGateway = clienteGateway;
        _servicoUseCase = servicoUseCase;
        _ordemServicoGateway = ordemServicoGateway;
        _eventosGateway = eventosGateway;
    }

    public async Task<OrdemServico> AtualizarUseCaseAsync(Guid id, AtualizarOrdemServicoUseCaseDto request)
    {
        var metodo = nameof(AtualizarUseCaseAsync);

        try
        {
            LogInicio(metodo, new { id, request });

            OrdemServico ordemServico = await _ordemServicoGateway.ObterPorIdAsync(id) ??
                throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

            ordemServico.Atualizar(
                request.ClienteId,
                request.VeiculoId,
                request.ServicoId,
                request.Descricao,
                request.Status);

            await _ordemServicoGateway.EditarAsync(ordemServico);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar a ordem de serviço");

            //TODO: refatorar isso
            if (ordemServico.Status == StatusOrdemServico.EmDiagnostico)
            {
                await _eventosGateway.Publicar(new OrdemServicoEmOrcamentoEventDTO(ordemServico.Id));
            }
            else if (ordemServico.Status == StatusOrdemServico.Cancelada)
            {
                await _eventosGateway.Publicar(new OrdemServicoCanceladaEventDTO(ordemServico.Id));
            }
            else if (ordemServico.Status == StatusOrdemServico.Finalizada)
            {
                await _eventosGateway.Publicar(new OrdemServicoFinalizadaEventDTO(ordemServico.Id));
            }

            LogFim(metodo, ordemServico);

            return ordemServico;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<OrdemServico> CadastrarUseCaseAsync(CadastrarOrdemServicoUseCaseDto request)
    {
        var metodo = nameof(CadastrarUseCaseAsync);

        try
        {
            LogInicio(metodo, request);

            await VerificarEntidadesRelacionadasAsync(request);

            OrdemServico ordemServico = new()
            {
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                ServicoId = request.ServicoId,
                Descricao = request.Descricao,
                Status = StatusOrdemServico.Recebida
            };

            var entidade = await _ordemServicoGateway.CadastrarAsync(ordemServico);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao cadastrar a ordem de serviço");

            LogFim(metodo, entidade);

            return entidade;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    private async Task VerificarEntidadesRelacionadasAsync(CadastrarOrdemServicoUseCaseDto request)
    {
        var cliente = await _clienteGateway.ObterClienteComVeiculoPorIdAsync(request.ClienteId)
            ?? throw new DadosNaoEncontradosException("O cliente não foi encontrado");

        var veiculo = cliente.Veiculos.FirstOrDefault(v => v.Id == request.VeiculoId);
        _ = veiculo ?? throw new DadosNaoEncontradosException("O veículo não foi encontrado");

        var servico = await _servicoUseCase.ObterServicoPorIdUseCaseAsync(request.ServicoId);
        if (!servico.Disponivel)
            throw new ServicoIndisponivelException("Serviço indisponível");
    }

    public async Task<OrdemServico?> ObterPorIdUseCaseAsync(Guid id)
    {
        var metodo = nameof(ObterPorIdUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var ordemServico = await _ordemServicoGateway.ObterOrdemServicoPorIdComInsumos(id);

            LogFim(metodo, ordemServico);
            return ordemServico;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorStatusUseCaseAsync(StatusOrdemServico status)
    {
        var metodo = nameof(ObterPorStatusUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var ordemServico = await _ordemServicoGateway.ObterOrdemServicoPorStatusAsync(status);

            LogFim(metodo, ordemServico);

            return ordemServico;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<IEnumerable<OrdemServico>> ObterTodosUseCaseAsync()
    {
        var metodo = nameof(ObterTodosUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var ordensServico = await _ordemServicoGateway.ObterTodosAsync();

            LogFim(metodo, ordensServico);

            return ordensServico;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task AceitarOrcamentoUseCaseAsync(Guid id)
    {
        var metodo = nameof(AceitarOrcamentoUseCaseAsync);

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

    public async Task RecusarOrcamentoUseCaseAsync(Guid id)
    {
        var metodo = nameof(RecusarOrcamentoUseCaseAsync);

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
        OrdemServico ordemServico = await _ordemServicoGateway.ObterPorIdAsync(id) ??
            throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

        await VerificarOrcamentoExpiradoAsync(ordemServico);

        if (aceito)
            ordemServico.Status = StatusOrdemServico.EmExecucao;
        else
        {
            ordemServico.Status = StatusOrdemServico.Cancelada;
            await _eventosGateway.Publicar(new OrdemServicoCanceladaEventDTO(id));
        }

        ordemServico.MarcarComoAtualizada();
        await _ordemServicoGateway.EditarAsync(ordemServico);

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
            ordemServico.MarcarComoAtualizada();

            await _ordemServicoGateway.EditarAsync(ordemServico);
            await Commit();

            throw new OrcamentoExpiradoException("Orçamento expirado");
        }
    }
}