using Aplicacao.Interfaces.Gateways;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.AtualizarEstoque
{
    public class AtualizarEstoqueUseCase(IEstoqueGateway gateway, IUnidadeDeTrabalho udt) : IAtualizarEstoqueUseCase
    {
        private readonly IEstoqueGateway gateway = gateway;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<Dominio.Entidades.Estoque> ExecutarAsync(Guid id, AtualizarEstoqueRequest request)
        {
            var estoque = await gateway.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Estoque não encontrado.");

            estoque.Atualizar(
                request.Insumo,
                request.Descricao,
                request.Preco,
                request.QuantidadeDisponivel,
                request.QuantidadeMinima
            );

            estoque.DataAtualizacao = DateTime.UtcNow;

            await gateway.EditarAsync(estoque);

            if (!await udt.Commit())
                throw new PersistirDadosException("Erro ao atualizar estoque");

            return estoque;
        }
    }
}
