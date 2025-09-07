using Aplicacao.Ports;
using Dominio.Exceptions;

namespace Aplicacao.UseCases.Estoque.ObterEstoque
{
    public class ObterEstoquePorIdUseCase(IEstoqueRepository repositorio) : IObterEstoquePorIdUseCase
    {
        private readonly IEstoqueRepository repositorio = repositorio;

        public async Task<EstoqueResponse> ExecuteAsync(Guid id)
        {
            var estoque = await repositorio.ObterPorIdAsync(id) 
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado.");

            return new EstoqueResponse
            {
                Id = estoque.Id,
                Insumo = estoque.Insumo,
                Descricao = estoque.Descricao,
                Preco = estoque.Preco,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                QuantidadeMinima = estoque.QuantidadeMinima,
                DataCadastro = estoque.DataCadastro,
                DataAtualizacao = estoque.DataAtualizacao
            };
        }
    }
}