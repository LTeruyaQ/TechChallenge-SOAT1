using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class EstoqueServico : ServicoAbstrato<EstoqueServico, Estoque>, IEstoqueServico
{
    private readonly IMapper _mapper;

    public EstoqueServico(
        IRepositorio<Estoque> repositorio,
        ILogServico<EstoqueServico> logServico,
        IUnidadeDeTrabalho uot,
        IMapper mapper)
        : base(repositorio, logServico, uot)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<EstoqueResponse> AtualizarAsync(Guid id, AtualizarEstoqueRequest request)
    {
        string metodo = nameof(AtualizarAsync);

        try
        {
            LogInicio(metodo, request);

            var estoque = await _repositorio.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            if (request.Insumo != null) estoque.Insumo = request.Insumo;
            if (request.Descricao != null) estoque.Descricao = request.Descricao;
            if (request.Preco.HasValue) estoque.Preco = request.Preco.Value;
            if (request.QuantidadeDisponivel.HasValue) estoque.QuantidadeDisponivel = request.QuantidadeDisponivel.Value;
            if (request.QuantidadeMinima.HasValue) estoque.QuantidadeMinima = request.QuantidadeMinima.Value;

            estoque.DataAtualizacao = DateTime.UtcNow;

            await _repositorio.EditarAsync(estoque);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar estoque");

            var response = _mapper.Map<EstoqueResponse>(estoque);
            LogFim(metodo, response);

            return response;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<EstoqueResponse> CadastrarAsync(CadastrarEstoqueRequest request)
    {
        string metodo = nameof(CadastrarAsync);

        try
        {
            LogInicio(metodo, request);

            var estoque = _mapper.Map<Estoque>(request);

            await _repositorio.CadastrarAsync(estoque);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao cadastrar estoque");

            var response = _mapper.Map<EstoqueResponse>(estoque);
            LogFim(metodo, response);

            return response;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<IEnumerable<EstoqueResponse>> ObterTodosAsync()
    {
        string metodo = nameof(ObterTodosAsync);

        try
        {
            LogInicio(metodo);

            var estoques = await _repositorio.ObterTodosAsync();
            var response = _mapper.Map<IEnumerable<EstoqueResponse>>(estoques);

            LogFim(metodo, response);

            return response;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<EstoqueResponse> ObterPorIdAsync(Guid id)
    {
        string metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            var estoque = await _repositorio.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            var response = _mapper.Map<EstoqueResponse>(estoque);
            LogFim(metodo, response);

            return response;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<bool> DeletarAsync(Guid id)
    {
        string metodo = nameof(DeletarAsync);

        try
        {
            LogInicio(metodo);

            var estoque = await _repositorio.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            await _repositorio.DeletarAsync(estoque);
            var sucesso = await Commit();

            LogFim(metodo, sucesso);

            return sucesso;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }
}