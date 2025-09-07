using Aplicacao.Interfaces.Gateways;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.DeletarEstoque
{
    public class DeletarEstoqueUseCase(IEstoqueGateway gateway, IUnidadeDeTrabalho udt) : IDeletarEstoqueUseCase
    {
        private readonly IEstoqueGateway gateway = gateway;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<bool> ExecutarAsync(Guid id)
        {
            var estoque = await gateway.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Estoque não encontrado.");

            await gateway.DeletarAsync(estoque);

            return await udt.Commit();
        }
    }
}
