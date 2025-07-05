using Dominio.DTOs.Estoque;
using Dominio.Entidades;

namespace Dominio.Interfaces.Servicos;

public interface IEstoqueServico
{
    Task<Estoque> CadastrarAsync(EstoqueRegistrarDto estoque);
    Task<Estoque> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(Guid id, EstoqueAtualizarDto estoqueDto);
    Task RemoverAsync(Guid id);
}
