using Core.UseCases.Estoques.ObterEstoque;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Estoques
{
    /// <summary>
    /// Interface para o handler de obtenção de estoque por ID
    /// </summary>
    public interface IObterEstoqueHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de estoque por ID
        /// </summary>
        /// <param name="id">ID do estoque a ser obtido</param>
        /// <returns>Resposta contendo o estoque encontrado</returns>
        Task<ObterEstoqueResponse> Handle(Guid id);
    }
}
