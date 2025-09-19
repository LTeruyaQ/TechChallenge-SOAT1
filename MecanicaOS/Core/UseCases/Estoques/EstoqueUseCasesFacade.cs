using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Interfaces.Handlers.Estoques;
using Core.Interfaces.UseCases;

namespace Core.UseCases.Estoques
{
    /// <summary>
    /// Facade completo para manter compatibilidade com a interface IEstoqueUseCases
    /// utilizando todos os novos casos de uso individuais
    /// </summary>
    public class EstoqueUseCasesFacade : IEstoqueUseCases
    {
        private readonly ICadastrarEstoqueHandler _cadastrarEstoqueHandler;
        private readonly IAtualizarEstoqueHandler _atualizarEstoqueHandler;
        private readonly IDeletarEstoqueHandler _deletarEstoqueHandler;
        private readonly IObterEstoqueHandler _obterEstoqueHandler;
        private readonly IObterTodosEstoquesHandler _obterTodosEstoquesHandler;
        private readonly IObterEstoqueCriticoHandler _obterEstoqueCriticoHandler;

        public EstoqueUseCasesFacade(
            ICadastrarEstoqueHandler cadastrarEstoqueHandler,
            IAtualizarEstoqueHandler atualizarEstoqueHandler,
            IDeletarEstoqueHandler deletarEstoqueHandler,
            IObterEstoqueHandler obterEstoqueHandler,
            IObterTodosEstoquesHandler obterTodosEstoquesHandler,
            IObterEstoqueCriticoHandler obterEstoqueCriticoHandler)
        {
            _cadastrarEstoqueHandler = cadastrarEstoqueHandler ?? throw new ArgumentNullException(nameof(cadastrarEstoqueHandler));
            _atualizarEstoqueHandler = atualizarEstoqueHandler ?? throw new ArgumentNullException(nameof(atualizarEstoqueHandler));
            _deletarEstoqueHandler = deletarEstoqueHandler ?? throw new ArgumentNullException(nameof(deletarEstoqueHandler));
            _obterEstoqueHandler = obterEstoqueHandler ?? throw new ArgumentNullException(nameof(obterEstoqueHandler));
            _obterTodosEstoquesHandler = obterTodosEstoquesHandler ?? throw new ArgumentNullException(nameof(obterTodosEstoquesHandler));
            _obterEstoqueCriticoHandler = obterEstoqueCriticoHandler ?? throw new ArgumentNullException(nameof(obterEstoqueCriticoHandler));
        }

        public async Task<Estoque> CadastrarUseCaseAsync(CadastrarEstoqueUseCaseDto request)
        {
            var response = await _cadastrarEstoqueHandler.Handle(request);
            return response.Estoque;
        }

        public async Task<Estoque> AtualizarUseCaseAsync(Guid id, AtualizarEstoqueUseCaseDto request)
        {
            var response = await _atualizarEstoqueHandler.Handle(id, request);
            return response.Estoque;
        }

        public async Task<bool> DeletarUseCaseAsync(Guid id)
        {
            var response = await _deletarEstoqueHandler.Handle(id);
            return response.Sucesso;
        }

        public async Task<Estoque> ObterPorIdUseCaseAsync(Guid id)
        {
            var response = await _obterEstoqueHandler.Handle(id);
            return response.Estoque;
        }

        public async Task<IEnumerable<Estoque>> ObterTodosUseCaseAsync()
        {
            var response = await _obterTodosEstoquesHandler.Handle();
            return response.Estoques;
        }

        public async Task<IEnumerable<Estoque>> ObterEstoqueCriticoUseCaseAsync()
        {
            var response = await _obterEstoqueCriticoHandler.Handle();
            return response.EstoquesCriticos;
        }
    }
}
