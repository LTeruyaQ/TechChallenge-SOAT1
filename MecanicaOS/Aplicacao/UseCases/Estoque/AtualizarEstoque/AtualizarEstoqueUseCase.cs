using Aplicacao.Interfaces.Gateways;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.AtualizarEstoque
{
    public class AtualizarEstoqueUseCase(IEstoqueGateway gateway, IUnidadeDeTrabalho udt) : IAtualizarEstoqueUseCase
    {
        private readonly IEstoqueGateway gateway = gateway;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<Dominio.Entidades.Estoque> ExecutarAsync(Dominio.Entidades.Estoque estoque)
        {
            estoque.DataAtualizacao = DateTime.UtcNow;

            await gateway.EditarAsync(estoque);

            if (!await udt.Commit())
                throw new PersistirDadosException("Erro ao atualizar estoque");

            return estoque;
        }
    }
}