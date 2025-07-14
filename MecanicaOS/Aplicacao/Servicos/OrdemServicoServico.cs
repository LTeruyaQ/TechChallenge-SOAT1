using Aplicacao.DTOs.Requests.OrdermServico;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class OrdemServicoServico : ServicoAbstrato<OrdemServicoServico, OrdemServico>, IOrdemServicoServico
{
    public OrdemServicoServico(
        IRepositorio<OrdemServico> repositorio,
        ILogServico<OrdemServicoServico> logServico,
        IUnidadeDeTrabalho uot, IMapper mapper) :
        base(repositorio, logServico, uot, mapper)
    {
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
                request.Orcamento, 
                request.Descricao, 
                request.Status);

            await _repositorio.EditarAsync(ordemServico);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar a ordem de serviço");

            LogFim(metodo, ordemServico);

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

    public async Task<OrdemServicoResponse?> ObterPorIdAsync(Guid id)
    {
        var metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            var ordemServico = await _repositorio.ObterPorIdAsync(id);

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
}