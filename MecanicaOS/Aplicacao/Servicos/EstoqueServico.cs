using Aplicacao.DTOs.Estoque;
using Aplicacao.Logs.Services;
using Aplicacao.Servicos.Abstrato;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Aplicacao.Servicos;

public class EstoqueServico : ServicoAbstratoLog<EstoqueServico>, IEstoqueServico
{
    private readonly ICrudRepositorio<Estoque> _repositorio;

    public EstoqueServico(
            ICorrelationIdService correlationIdLog,
            ILogger<EstoqueServico> logger,
            ICrudRepositorio<Estoque> repositorio) : base(correlationIdLog, logger)
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

            await _repositorio.Editar(estoque);

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

            LogFim(metodo);

            return await _repositorio.CadastrarAsync(estoque);
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

            IEnumerable<Estoque> result = await _repositorio.ObterTodos();

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

            Estoque? estoque = await _repositorio.ObterPorIdAsync(id);

            LogFim(metodo, estoque);

            return estoque is null ? throw new EntidadeNaoEncontradaException("Estoque não encontrado.") : estoque;
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