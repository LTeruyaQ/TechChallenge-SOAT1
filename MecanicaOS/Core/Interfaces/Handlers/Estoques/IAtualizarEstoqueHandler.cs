using Core.DTOs.UseCases.Estoque;
using Core.UseCases.Estoques.AtualizarEstoque;

namespace Core.Interfaces.Handlers.Estoques
{
    /// <summary>
    /// Interface para o handler de atualização de estoque
    /// </summary>
    public interface IAtualizarEstoqueHandler
    {
        /// <summary>
        /// Manipula a operação de atualização de estoque
        /// </summary>
        /// <param name="id">ID do estoque a ser atualizado</param>
        /// <param name="request">DTO com os dados atualizados do estoque</param>
        /// <returns>Resposta contendo o estoque atualizado</returns>
        Task<AtualizarEstoqueResponse> Handle(Guid id, AtualizarEstoqueUseCaseDto request);
    }
}
