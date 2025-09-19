using Adapters.Presenters;
using Core.DTOs.Requests.Autenticacao;
using Core.DTOs.Responses.Autenticacao;
using Core.DTOs.UseCases.Autenticacao;
using Core.Interfaces.Controllers;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class AutenticacaoController : IAutenticacaoController
    {
        private readonly IAutenticacaoUseCases _autenticacaoUseCases;
        private readonly IAutenticacaoPresenter _autenticacaoPresenter;

        public AutenticacaoController(ICompositionRoot compositionRoot)
        {
            _autenticacaoUseCases = compositionRoot.CriarAutenticacaoUseCases();
            _autenticacaoPresenter = new AutenticacaoPresenter();
        }

        public async Task<AutenticacaoResponse> AutenticarAsync(AutenticacaoRequest autenticacaoRequest)
        {
            var useCaseDto = MapearParaAutenticacaoUseCaseDto(autenticacaoRequest);
            var autenticacaoDto = await _autenticacaoUseCases.AutenticarUseCaseAsync(useCaseDto);
            return _autenticacaoPresenter.ParaResponse(autenticacaoDto);
        }

        internal AutenticacaoUseCaseDto MapearParaAutenticacaoUseCaseDto(AutenticacaoRequest request)
        {
            if (request is null)
                return null;

            return new AutenticacaoUseCaseDto
            {
                Email = request.Email,
                Senha = request.Senha
            };
        }
    }
}
