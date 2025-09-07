using Aplicacao.Ports;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.CriarEstoque
{
    public class CriarEstoqueUseCase(IEstoqueRepository repositorio, IUnidadeDeTrabalho udt) : ICriarEstoqueUseCase
    {
        private readonly IEstoqueRepository repositorio = repositorio;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<EstoqueResponse> ExecuteAsync(CriarEstoqueRequest request)
        {
            var estoque = new Dominio.Entidades.Estoque
            (
                request.Insumo,
                request.Descricao,
                request.Preco,
                request.QuantidadeDisponivel,
                request.QuantidadeMinima
            );

            await repositorio.CadastrarAsync(estoque);

            if (!await udt.Commit())
                throw new PersistirDadosException("Erro ao cadastrar estoque");

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
