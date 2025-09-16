using Adapters.DTOs.Requests.Servico;
using Adapters.DTOs.Responses.Servico;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.Servico;
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

        public async Task<IEnumerable<ServicoResponse>> ObterServicosDisponiveis()
        {
            return _servicoPresenter.ParaResponse(await _servicoUseCases.ObterServicosDisponiveisUseCaseAsync());
        }

        public async Task<ServicoResponse> ObterPorId(Guid id)
        {
            return _servicoPresenter.ParaResponse(await _servicoUseCases.ObterServicoPorIdUseCaseAsync(id));
        }

        public async Task<ServicoResponse> Criar(CadastrarServicoRequest cadastrarServicoRequest)
        {
            var useCaseDto = MapearParaCadastrarServicoUseCaseDto(cadastrarServicoRequest);
            var resultado = await _servicoUseCases.CadastrarServicoUseCaseAsync(useCaseDto);
            return _servicoPresenter.ParaResponse(resultado);
        }
        
        internal CadastrarServicoUseCaseDto MapearParaCadastrarServicoUseCaseDto(CadastrarServicoRequest request)
        {
            if (request is null)
                return null;

            return new CadastrarServicoUseCaseDto
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Valor = request.Valor,
                Disponivel = request.Disponivel
            };
        }

        public async Task<ServicoResponse> Atualizar(Guid id, EditarServicoRequest atualizarServicoRequest)
        {
            var useCaseDto = MapearParaEditarServicoUseCaseDto(atualizarServicoRequest);
            var resultado = await _servicoUseCases.EditarServicoUseCaseAsync(id, useCaseDto);
            return _servicoPresenter.ParaResponse(resultado);
        }
        
        internal EditarServicoUseCaseDto MapearParaEditarServicoUseCaseDto(EditarServicoRequest request)
        {
            if (request is null)
                return null;

            return new EditarServicoUseCaseDto
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Valor = request.Valor,
                Disponivel = request.Disponivel
            };
        }

        public async Task Deletar(Guid id)
        {
            await _servicoUseCases.DeletarServicoUseCaseAsync(id);
        }
    }
}
