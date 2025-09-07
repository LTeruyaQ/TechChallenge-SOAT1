using Aplicacao.Interfaces.Gateways;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.CriarEstoque
{
    public class CriarEstoqueUseCase(IEstoqueGateway gateway, IUnidadeDeTrabalho udt) : ICriarEstoqueUseCase
    {
        private readonly IEstoqueGateway gateway = gateway;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<Dominio.Entidades.Estoque> ExecutarAsync(CriarEstoqueRequest request)
        {
            var estoque = new Dominio.Entidades.Estoque
            (
                request.Insumo,
                request.Descricao,
                request.Preco,
                request.QuantidadeDisponivel,
                request.QuantidadeMinima
            );

            await gateway.CadastrarAsync(estoque);

            if (!await udt.Commit())
                throw new PersistirDadosException("Erro ao cadastrar estoque");

            return estoque;
        }
    }
}
