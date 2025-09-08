using Core.Entidades;
using Core.Especificacoes.Estoque;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class EstoqueGateway : IEstoqueGateway
    {
        private readonly IRepositorio<Estoque> _repositorioEstoque;

        public EstoqueGateway(IRepositorio<Estoque> repositorioEstoque)
        {
            _repositorioEstoque = repositorioEstoque;
        }

        public async Task CadastrarAsync(Estoque estoque)
        {
            await _repositorioEstoque.CadastrarAsync(estoque);
        }

        public async Task DeletarAsync(Estoque estoque)
        {
            await _repositorioEstoque.DeletarAsync(estoque);
        }

        public async Task EditarAsync(Estoque estoque)
        {
            await _repositorioEstoque.EditarAsync(estoque);
        }

        public async Task<IEnumerable<Estoque>> ObterEstoqueCriticoAsync()
        {
            var especificacao = new ObterEstoqueCriticoEspecificacao();
            return await _repositorioEstoque.ListarAsync(especificacao);
        }

        public async Task<Estoque?> ObterPorIdAsync(Guid id)
        {
            return await _repositorioEstoque.ObterPorIdAsync(id);
        }

        public async Task<IEnumerable<Estoque>> ObterTodosAsync()
        {
            return await _repositorioEstoque.ObterTodosAsync();
        }
    }
}
