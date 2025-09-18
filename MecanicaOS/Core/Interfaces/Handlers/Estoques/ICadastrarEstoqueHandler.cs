using Core.DTOs.UseCases.Estoque;
using Core.UseCases.Estoques.CadastrarEstoque;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Estoques
{
    /// <summary>
    /// Interface para o handler de cadastro de estoque
    /// </summary>
    public interface ICadastrarEstoqueHandler
    {
        /// <summary>
        /// Manipula a operação de cadastro de estoque
        /// </summary>
        /// <param name="request">DTO com os dados do estoque a ser cadastrado</param>
        /// <returns>Resposta contendo o estoque cadastrado</returns>
        Task<CadastrarEstoqueResponse> Handle(CadastrarEstoqueUseCaseDto request);
    }
}
