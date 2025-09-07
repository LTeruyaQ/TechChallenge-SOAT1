using Aplicacao.Interfaces.Gateways;
using Dominio.Exceptions;

namespace Aplicacao.UseCases.Estoque.ObterEstoque
{
    public class ObterEstoquePorIdUseCase(IEstoqueGateway gateway) : IObterEstoquePorIdUseCase
    {
        private readonly IEstoqueGateway gateway = gateway;

        public async Task<Dominio.Entidades.Estoque> ExecutarAsync(Guid id)
        {
            var estoque = await gateway.ObterPorIdAsync(id) 
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado.");

            return estoque;
        }
    }
}