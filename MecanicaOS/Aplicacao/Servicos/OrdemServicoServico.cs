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
        IUnidadeDeTrabalho udt,
        IMapper mapper,
        IMediator mediator,
        IRepositorio<Cliente> clienteRepositorio,
        IServicoServico servicoServico,
        IUsuarioLogadoServico usuarioLogadoServico) :
        base(repositorio, logServico, udt, mapper, usuarioLogadoServico)
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

            var response = _mapper.Map<OrdemServicoResponse>(ordemServico);

            LogFim(metodo, response);

            return response;
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

            var response = _mapper.Map<OrdemServicoResponse>(entidade);
            
            LogFim(metodo, response);

            return response;
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

        var servico = await _servicoServico.ObterServicoPorIdAsync(request.ServicoId);
        if (!servico.Disponivel)
            throw new ServicoIndisponivelException("Serviço indisponível");
    }

    public async Task<OrdemServicoResponse?> ObterPorIdAsync(Guid id)
    {
        var metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            var especificacao = new ObterOrdemServicoPorIdComInsumosEOrcamentoEspecificacao(id);
            var ordemServico = await _repositorio.ObterUmSemRastreamentoAsync(especificacao);
            var response = _mapper.Map<OrdemServicoResponse>(ordemServico);

            LogFim(metodo, response);
            return response;
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

            var ordemServico = await _repositorio.ListarAsync(filtroOSStatus);

            var response = _mapper.Map<IEnumerable<OrdemServicoResponse>>(ordemServico);

            LogFim(metodo, response);

            return response;
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

            var response = _mapper.Map<IEnumerable<OrdemServicoResponse>>(ordensServico);

            LogFim(metodo, response);
            
            return response;
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
        var filtroOS = new ObterOrdemServicoPorIdComOrcamentoEspecificacao(id);
        OrdemServico ordemServico = await _repositorio.ObterUmAsync(filtroOS) ??
            throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");
        
        ordemServico.ValidarOrcamentoPresente();

        await VerificarOrcamentoExpiradoAsync(ordemServico);
        
        if (aceito)
            ordemServico.AprovarOrcamento();
        else
        {
            ordemServico.Cancelar();
            await _mediator.Publish(new OrdemServicoCanceladaEvent(id));
        }

        await _repositorio.EditarAsync(ordemServico);

        if (!await Commit())
            throw new PersistirDadosException($"Erro ao {(aceito ? "aceitar" : "recusar")} o orçamento da ordem de serviço");
    }

    private async Task VerificarOrcamentoExpiradoAsync(OrdemServico ordemServico)
    {
        if (ordemServico.OrcamentoExpirou())
            throw new OrcamentoExpiradoException("Orçamento expirado");
        
        ordemServico.ValidarStatusAguardando();
        
        if (ordemServico.DeveExpirar())
            await ExpirarOrcamentoAsync(ordemServico);
    }

    private async Task ExpirarOrcamentoAsync(OrdemServico ordemServico)
    {
        ordemServico.Expirar();

        await _repositorio.EditarAsync(ordemServico);
        
        if (!await Commit())
            throw new PersistirDadosException("Erro ao expirar o orçamento da ordem de serviço");

        throw new OrcamentoExpiradoException("Orçamento expirado");
    }
}