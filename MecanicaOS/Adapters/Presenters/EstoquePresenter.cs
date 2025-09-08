using Adapters.DTOs.Requests.Estoque;
using Adapters.Presenters.Interfaces;
using Core.DTOs.Estoque;

namespace Adapters.Presenters
{
    public class EstoquePresenter : IEstoquePresenter
    {
        public CadastrarEstoqueUseCaseDto ParaUseCaseDto(CadastrarEstoqueRequest request)
        {
            if (request == null)
                return null;

            return new CadastrarEstoqueUseCaseDto
            {
                Insumo = request.Insumo,
                Descricao = request.Descricao,
                Preco = request.Preco,
                QuantidadeDisponivel = request.QuantidadeDisponivel,
                QuantidadeMinima = request.QuantidadeMinima
            };
        }

        public AtualizarEstoqueUseCaseDto ParaUseCaseDto(AtualizarEstoqueRequest request)
        {
            if (request == null)
                return null;

            return new AtualizarEstoqueUseCaseDto
            {
                Insumo = request.Insumo,
                Descricao = request.Descricao,
                Preco = request.Preco,
                QuantidadeDisponivel = request.QuantidadeDisponivel,
                QuantidadeMinima = request.QuantidadeMinima
            };
        }
    }
}
