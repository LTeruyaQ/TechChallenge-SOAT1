using Aplicacao.Interfaces.Gateways;
using Dominio.Exceptions;

namespace Aplicacao.UseCases.Estoque.ListaEstoque
{
    public class ListarEstoqueUseCase(IEstoqueGateway gateway) : IListarEstoqueUseCase
    {
        private readonly IEstoqueGateway gateway = gateway;

        public async Task<IEnumerable<Dominio.Entidades.Estoque>> ExecutarAsync()
        {
            var estoques = await gateway.ObterTodosAsync();

            return estoques;
        }
    }
}
