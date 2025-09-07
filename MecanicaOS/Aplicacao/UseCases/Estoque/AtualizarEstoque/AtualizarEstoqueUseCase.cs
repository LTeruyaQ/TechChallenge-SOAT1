using Aplicacao.Ports;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.AtualizarEstoque
{
    public class AtualizarEstoqueUseCase(IEstoqueRepository repositorio, IUnidadeDeTrabalho udt) : IAtualizarEstoqueUseCase
    {
        private readonly IEstoqueRepository repositorio = repositorio;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<EstoqueResponse> ExecuteAsync(Guid id, AtualizarEstoqueRequest request)
        {
            var estoque = await repositorio.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Estoque não encontrado.");

            estoque.Atualizar(
                request.Insumo,
                request.Descricao,
                request.Preco,
                request.QuantidadeDisponivel,
                request.QuantidadeMinima
            );

            estoque.DataAtualizacao = DateTime.UtcNow;

            await repositorio.EditarAsync(estoque);

            if (!await udt.Commit())
                throw new PersistirDadosException("Erro ao atualizar estoque");

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
