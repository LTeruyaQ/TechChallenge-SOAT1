using Adapters.DTOs.Requests.Servico;
using Adapters.DTOs.Responses.Servico;
using Adapters.Presenters.Interfaces;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class ServicoController
    {
        private readonly IServicoUseCases _servicoUseCases;
        private readonly IServicoPresenter _servicoPresenter;

        public ServicoController(IServicoUseCases servicoUseCases, IServicoPresenter servicoPresenter)
        {
            _servicoUseCases = servicoUseCases;
            _servicoPresenter = servicoPresenter;
        }

        public async Task<IEnumerable<ServicoResponse>> ObterTodos()
        {
            return _servicoPresenter.ParaResponse(await _servicoUseCases.ObterTodosUseCaseAsync());
        }

        public async Task<IEnumerable<ServicoResponse>> ObterServicosDisponiveis(int id)
        {
            return _servicoPresenter.ParaResponse(await _servicoUseCases.ObterServicosDisponiveisUseCaseAsync());
        }

        public async Task<ServicoResponse> ObterPorId(Guid id)
        {
            return _servicoPresenter.ParaResponse(await _servicoUseCases.ObterServicoPorIdUseCaseAsync(id));
        }

        public async Task<ServicoResponse> Criar(CadastrarServicoRequest cadastrarServicoRequest)
        {
            return _servicoPresenter.ParaResponse(
                await _servicoUseCases.CadastrarServicoUseCaseAsync(
                    _servicoPresenter.ParaUseCaseDto(cadastrarServicoRequest)));
        }

        public async Task<ServicoResponse> Atualizar(Guid id, EditarServicoRequest atualizarServicoRequest)
        {
            return _servicoPresenter.ParaResponse(
                await _servicoUseCases.EditarServicoUseCaseAsync(id,
                    _servicoPresenter.ParaUseCaseDto(atualizarServicoRequest)));
        }

        public async Task Deletar(Guid id)
        {
            await _servicoUseCases.DeletarServicoUseCaseAsync(id);
        }
    }
}
