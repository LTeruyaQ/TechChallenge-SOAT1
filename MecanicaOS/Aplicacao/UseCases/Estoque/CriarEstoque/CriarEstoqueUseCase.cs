using Aplicacao.Interfaces.Gateways;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.CriarEstoque
{
    public class CriarEstoqueUseCase(IEstoqueGateway gateway, IUnidadeDeTrabalho udt) : ICriarEstoqueUseCase
    {
        private readonly IEstoqueGateway gateway = gateway;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<Dominio.Entidades.Estoque> ExecutarAsync(Dominio.Entidades.Estoque estoque)
        {
            await gateway.CadastrarAsync(estoque);

            if (!await udt.Commit())
                throw new PersistirDadosException("Erro ao cadastrar estoque");

            return estoque;
        }
    }
}