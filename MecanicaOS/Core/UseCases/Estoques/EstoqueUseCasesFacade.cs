using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Interfaces.UseCases;
using Core.UseCases.Estoques.AtualizarEstoque;
using Core.UseCases.Estoques.CadastrarEstoque;
using Core.UseCases.Estoques.DeletarEstoque;
using Core.UseCases.Estoques.ObterEstoque;
using Core.UseCases.Estoques.ObterEstoqueCritico;
using Core.UseCases.Estoques.ObterTodosEstoques;

namespace Core.UseCases.Estoques
{
    /// <summary>
    /// Facade completo para manter compatibilidade com a interface IEstoqueUseCases
    /// utilizando todos os novos casos de uso individuais
    /// </summary>
    public class EstoqueUseCasesFacade : IEstoqueUseCases
    {
        private readonly CadastrarEstoqueHandler _cadastrarEstoqueHandler;
        private readonly AtualizarEstoqueHandler _atualizarEstoqueHandler;
        private readonly DeletarEstoqueHandler _deletarEstoqueHandler;
        private readonly ObterEstoqueHandler _obterEstoqueHandler;
        private readonly ObterTodosEstoquesHandler _obterTodosEstoquesHandler;
        private readonly ObterEstoqueCriticoHandler _obterEstoqueCriticoHandler;

        public EstoqueUseCasesFacade(
            CadastrarEstoqueHandler cadastrarEstoqueHandler,
            AtualizarEstoqueHandler atualizarEstoqueHandler,
            DeletarEstoqueHandler deletarEstoqueHandler,
            ObterEstoqueHandler obterEstoqueHandler,
            ObterTodosEstoquesHandler obterTodosEstoquesHandler,
            ObterEstoqueCriticoHandler obterEstoqueCriticoHandler)
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
            var command = new CadastrarEstoqueCommand(request);
            var response = await _cadastrarEstoqueHandler.Handle(command);
            return response.Estoque;
        }

        public async Task<Estoque> AtualizarUseCaseAsync(Guid id, AtualizarEstoqueUseCaseDto request)
        {
            var command = new AtualizarEstoqueCommand(id, request);
            var response = await _atualizarEstoqueHandler.Handle(command);
            return response.Estoque;
        }

        public async Task<bool> DeletarUseCaseAsync(Guid id)
        {
            var command = new DeletarEstoqueCommand(id);
            var response = await _deletarEstoqueHandler.Handle(command);
            return response.Sucesso;
        }

        public async Task<Estoque> ObterPorIdUseCaseAsync(Guid id)
        {
            var command = new ObterEstoqueCommand(id);
            var response = await _obterEstoqueHandler.Handle(command);
            return response.Estoque;
        }

        public async Task<IEnumerable<Estoque>> ObterTodosUseCaseAsync()
        {
            var command = new ObterTodosEstoquesCommand();
            var response = await _obterTodosEstoquesHandler.Handle(command);
            return response.Estoques;
        }

        public async Task<IEnumerable<Estoque>> ObterEstoqueCriticoUseCaseAsync()
        {
            var command = new ObterEstoqueCriticoCommand();
            var response = await _obterEstoqueCriticoHandler.Handle(command);
            return response.EstoquesCriticos;
        }
    }
}
