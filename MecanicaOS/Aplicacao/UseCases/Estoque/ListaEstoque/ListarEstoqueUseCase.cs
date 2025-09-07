using Aplicacao.Ports;
using Dominio.Exceptions;

namespace Aplicacao.UseCases.Estoque.ListaEstoque
{
    public class ListarEstoqueUseCase(IEstoqueRepository repositorio) : IListarEstoqueUseCase
    {
        private readonly IEstoqueRepository repositorio = repositorio;

        public async Task<List<EstoqueResponse>> ExecuteAsync()
        {
            var estoques = await repositorio.ObterTodosAsync()
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado.");

            List<EstoqueResponse> response = [];

            foreach (var estoque in estoques)
            {
                response.Add(new EstoqueResponse
                {
                    Id = estoque.Id,
                    Insumo = estoque.Insumo,
                    Descricao = estoque.Descricao,
                    Preco = estoque.Preco,
                    QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                    QuantidadeMinima = estoque.QuantidadeMinima,
                    DataCadastro = estoque.DataCadastro,
                    DataAtualizacao = estoque.DataAtualizacao
                });
            }

            return response;
        }
    }
}
