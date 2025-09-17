using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Interfaces.UseCases;
using Core.UseCases.Servicos.CadastrarServico;
using Core.UseCases.Servicos.DeletarServico;
using Core.UseCases.Servicos.EditarServico;
using Core.UseCases.Servicos.ObterServico;
using Core.UseCases.Servicos.ObterServicoPorNome;
using Core.UseCases.Servicos.ObterServicosDisponiveis;
using Core.UseCases.Servicos.ObterTodosServicos;

namespace Core.UseCases.Servicos
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IServicoUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class ServicoUseCasesFacade : IServicoUseCases
    {
        private readonly CadastrarServicoHandler _cadastrarServicoHandler;
        private readonly EditarServicoHandler _editarServicoHandler;
        private readonly DeletarServicoHandler _deletarServicoHandler;
        private readonly ObterServicoHandler _obterServicoHandler;
        private readonly ObterTodosServicosHandler _obterTodosServicosHandler;
        private readonly ObterServicoPorNomeHandler _obterServicoPorNomeHandler;
        private readonly ObterServicosDisponiveisHandler _obterServicosDisponiveisHandler;

        public ServicoUseCasesFacade(
            CadastrarServicoHandler cadastrarServicoHandler,
            EditarServicoHandler editarServicoHandler,
            DeletarServicoHandler deletarServicoHandler,
            ObterServicoHandler obterServicoHandler,
            ObterTodosServicosHandler obterTodosServicosHandler,
            ObterServicoPorNomeHandler obterServicoPorNomeHandler,
            ObterServicosDisponiveisHandler obterServicosDisponiveisHandler)
        {
            _cadastrarServicoHandler = cadastrarServicoHandler ?? throw new ArgumentNullException(nameof(cadastrarServicoHandler));
            _editarServicoHandler = editarServicoHandler ?? throw new ArgumentNullException(nameof(editarServicoHandler));
            _deletarServicoHandler = deletarServicoHandler ?? throw new ArgumentNullException(nameof(deletarServicoHandler));
            _obterServicoHandler = obterServicoHandler ?? throw new ArgumentNullException(nameof(obterServicoHandler));
            _obterTodosServicosHandler = obterTodosServicosHandler ?? throw new ArgumentNullException(nameof(obterTodosServicosHandler));
            _obterServicoPorNomeHandler = obterServicoPorNomeHandler ?? throw new ArgumentNullException(nameof(obterServicoPorNomeHandler));
            _obterServicosDisponiveisHandler = obterServicosDisponiveisHandler ?? throw new ArgumentNullException(nameof(obterServicosDisponiveisHandler));
        }

        public async Task<Servico> CadastrarServicoUseCaseAsync(CadastrarServicoUseCaseDto request)
        {
            var command = new CadastrarServicoCommand(request);
            var response = await _cadastrarServicoHandler.Handle(command);
            return response.Servico;
        }

        public async Task DeletarServicoUseCaseAsync(Guid id)
        {
            var command = new DeletarServicoCommand(id);
            await _deletarServicoHandler.Handle(command);
        }

        public async Task<Servico> EditarServicoUseCaseAsync(Guid id, EditarServicoUseCaseDto request)
        {
            var command = new EditarServicoCommand(id, request);
            var response = await _editarServicoHandler.Handle(command);
            return response.Servico;
        }

        public async Task<Servico> ObterServicoPorIdUseCaseAsync(Guid id)
        {
            var useCase = new ObterServicoUseCase(id);
            var response = await _obterServicoHandler.Handle(useCase);
            return response.Servico!;
        }

        public async Task<Servico?> ObterServicoPorNomeUseCaseAsync(string nome)
        {
            var useCase = new ObterServicoPorNomeUseCase(nome);
            var response = await _obterServicoPorNomeHandler.Handle(useCase);
            return response.Servico;
        }

        public async Task<IEnumerable<Servico>> ObterServicosDisponiveisUseCaseAsync()
        {
            var useCase = new ObterServicosDisponiveisUseCase();
            var response = await _obterServicosDisponiveisHandler.Handle(useCase);
            return response.Servicos;
        }

        public async Task<IEnumerable<Servico>> ObterTodosUseCaseAsync()
        {
            var useCase = new ObterTodosServicosUseCase();
            var response = await _obterTodosServicosHandler.Handle(useCase);
            return response.Servicos;
        }
    }
}
