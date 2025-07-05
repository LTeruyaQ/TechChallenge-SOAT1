using Aplicacao.DTOs.Estoque;
using Aplicacao.Interfaces;
using Aplicacao.Servicos.Abstrato;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;

namespace Aplicacao.Servicos;

public class EstoqueServico : ServicoAbstrato<EstoqueServico>, IEstoqueServico
{
    private readonly ICrudRepositorio<Estoque> _repositorio;

    public EstoqueServico(ILogServico<EstoqueServico> logServico, ICrudRepositorio<Estoque> repositorio, IUnidadeDeTrabalho uot)
        : base(logServico, uot)
    {
        _repositorio = repositorio;
    }

    public async Task AtualizarAsync(Guid id, EstoqueAtualizarDto estoqueDto)
    {
        string metodo = nameof(AtualizarAsync);

        try
        {
            LogInicio(metodo, estoqueDto);

            Estoque estoque = await ObterPorIdAsync(id);

            estoque.Insumo = estoqueDto.Insumo ?? estoque.Insumo;
            estoque.Descricao = estoqueDto.Descricao ?? estoque.Descricao;
            estoque.Preco = estoqueDto.Preco ?? estoque.Preco;
            estoque.QuantidadeDisponivel = estoqueDto.QuantidadeDisponivel ?? estoque.QuantidadeDisponivel;
            estoque.QuantidadeMinima = estoqueDto.QuantidadeMinima ?? estoque.QuantidadeMinima;
            estoque.DataAtualizacao = DateTime.UtcNow;

            await _repositorio.EditarAsync(estoque);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar estoque");

            LogFim(metodo);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<Estoque> CadastrarAsync(EstoqueRegistrarDto estoqueDto)
    {
        string metodo = nameof(CadastrarAsync);

        try
        {
            LogInicio(metodo, estoqueDto);

            Estoque estoque = new()
            {
                Insumo = estoqueDto.Insumo,
                Descricao = estoqueDto.Descricao,
                Preco = estoqueDto.Preco,
                QuantidadeDisponivel = estoqueDto.QuantidadeDisponivel,
                QuantidadeMinima = estoqueDto.QuantidadeMinima,
            };

            var novoEstoque = await _repositorio.CadastrarAsync(estoque);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao cadastrar estoque");

            LogFim(metodo, novoEstoque);

            return novoEstoque;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<IEnumerable<Estoque>> ListarEstoquesAsync()
    {
        string metodo = nameof(ListarEstoquesAsync);

        try
        {
            LogInicio(metodo);

            IEnumerable<Estoque> result = await _repositorio.ObterTodosAsync();

            LogFim(metodo, result);

            return result;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<Estoque> ObterPorIdAsync(Guid id)
    {
        string metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            Estoque? estoque = await _repositorio.ObterPorIdAsync(id) ?? throw new EntidadeNaoEncontradaException("Estoque não encontrado.");

            LogFim(metodo, estoque);

            return estoque;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task RemoverAsync(Guid id)
    {
        string metodo = nameof(RemoverAsync);

        try
        {
            LogInicio(metodo);

            Estoque estoque = await ObterPorIdAsync(id);

            LogFim(metodo, estoque);

            await _repositorio.DeletarAsync(estoque);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }
}