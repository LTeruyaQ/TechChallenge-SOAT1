using Aplicacao.DTOs.Estoque;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;

namespace Aplicacao.Servicos;

public class EstoqueServico : IEstoqueServico
{
    private readonly ICrudRepositorio<Estoque> _repositorio;

    public EstoqueServico(ICrudRepositorio<Estoque> repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task AtualizarAsync(EstoqueAtualizarDto estoqueDto)
    {
        Estoque estoque = await ObterPorIdAsync(estoqueDto.Id);

        estoque.Insumo = estoqueDto.Insumo ?? estoque.Insumo;
        estoque.Descricao = estoqueDto.Descricao ?? estoque.Descricao;
        estoque.QuantidadeDisponivel = estoqueDto.QuantidadeDisponivel ?? estoque.QuantidadeDisponivel;
        estoque.QuantidadeMinima = estoqueDto.QuantidadeMinima ?? estoque.QuantidadeMinima;
        estoque.DataAtualizacao = DateTime.Now;

        await _repositorio.Editar(estoque);
    }

    public async Task<Estoque> CadastrarAsync(EstoqueRegistrarDto estoqueDto)
    {
        Estoque estoque = new()
        {
            Insumo = estoqueDto.Insumo,
            Descricao = estoqueDto.Descricao,
            QuantidadeDisponivel = estoqueDto.QuantidadeDisponivel,
            QuantidadeMinima = estoqueDto.QuantidadeMinima,
            DataCadastro = DateTime.Now,
            DataAtualizacao = DateTime.Now
        };

        return await _repositorio.CadastrarAsync(estoque);
    }

    public async Task<IEnumerable<Estoque>> ListarEstoquesAsync()
    {
        return await _repositorio.ObterTodos();
    }

    public async Task<Estoque> ObterPorIdAsync(Guid id)
    {
        Estoque? estoque = await _repositorio.ObterPorIdAsync(id);

        return estoque is null ? throw new EntidadeNaoEncontradaException("Estoque não encontrado.") : estoque;
    }

    public async Task RemoverAsync(Guid id)
    {
        Estoque estoque = await ObterPorIdAsync(id);

        await _repositorio.DeletarAsync(estoque);
    }
}