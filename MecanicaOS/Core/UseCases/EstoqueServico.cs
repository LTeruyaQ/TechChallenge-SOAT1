using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases;

public class EstoqueUseCases : UseCasesAbstrato<EstoqueUseCases, Estoque>, IEstoqueUseCases
{

    private readonly IEstoqueGateway _estoqueGateway;
    public EstoqueUseCases(
        IEstoqueGateway estoqueGateway,
        ILogServico<EstoqueUseCases> logServico,
        IUnidadeDeTrabalho udt,
        IUsuarioLogadoServico usuarioLogadoServico)
        : base(logServico, udt, usuarioLogadoServico)
    {
        _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
    }

    public async Task<Estoque> AtualizarUseCaseAsync(Guid id, AtualizarEstoqueUseCaseDto request)
    {
        string metodo = nameof(AtualizarUseCaseAsync);

        try
        {
            LogInicio(metodo, request);

            var estoque = await _estoqueGateway.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            if (request.Insumo != null) estoque.Insumo = request.Insumo;
            if (request.Descricao != null) estoque.Descricao = request.Descricao;
            if (request.Preco.HasValue) estoque.Preco = request.Preco.Value;
            if (request.QuantidadeDisponivel.HasValue) estoque.QuantidadeDisponivel = request.QuantidadeDisponivel.Value;
            if (request.QuantidadeMinima.HasValue) estoque.QuantidadeMinima = request.QuantidadeMinima.Value;

            estoque.DataAtualizacao = DateTime.UtcNow;

            await _estoqueGateway.EditarAsync(estoque);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar estoque");

            LogFim(metodo, estoque);

            return estoque;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<Estoque> CadastrarUseCaseAsync(CadastrarEstoqueUseCaseDto request)
    {
        string metodo = nameof(CadastrarUseCaseAsync);

        try
        {
            LogInicio(metodo, request);

            Estoque estoque = new()
            {
                Insumo = request.Insumo,
                Descricao = request.Descricao,
                Preco = request.Preco,
                QuantidadeDisponivel = request.QuantidadeDisponivel,
                QuantidadeMinima = request.QuantidadeMinima,
            };

            await _estoqueGateway.CadastrarAsync(estoque);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao cadastrar estoque");

            LogFim(metodo, estoque);

            return estoque;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<IEnumerable<Estoque>> ObterTodosUseCaseAsync()
    {
        string metodo = nameof(ObterTodosUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var estoques = await _estoqueGateway.ObterTodosAsync();

            LogFim(metodo, estoques);

            return estoques;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<Estoque> ObterPorIdUseCaseAsync(Guid id)
    {
        string metodo = nameof(ObterPorIdUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var estoque = await _estoqueGateway.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            LogFim(metodo, estoque);

            return estoque;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<bool> DeletarUseCaseAsync(Guid id)
    {
        string metodo = nameof(DeletarUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var estoque = await _estoqueGateway.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            await _estoqueGateway.DeletarAsync(estoque);
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