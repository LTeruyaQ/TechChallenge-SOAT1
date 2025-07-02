using Dominio.Entidades;
using Aplicacao.DTOs.Estoque;

namespace Dominio.Interfaces.Services;

public interface IEstoqueServico
{
    Task<Estoque> CadastrarAsync(EstoqueRegistrarDto estoque);
    Task<Estoque> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(Guid id, EstoqueAtualizarDto estoqueDto);
    Task RemoverAsync(Guid id);
}
