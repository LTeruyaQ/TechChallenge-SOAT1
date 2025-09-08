using Adapters.DTOs.Requests.Autenticacao;
using Adapters.DTOs.Responses.Autenticacao;
using Adapters.Presenters.Interfaces;
using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class AutenticacaoController
    {
        private readonly IAutenticacaoUseCases _autenticacaoUseCases;
        private readonly IAutenticacaoPresenter _autenticacaoPresenter;

        public AutenticacaoController(IAutenticacaoUseCases autenticacaoUseCases, IAutenticacaoPresenter autenticacaoPresenter)
        {
            _autenticacaoUseCases = autenticacaoUseCases;
            _autenticacaoPresenter = autenticacaoPresenter;
        }

        public async Task<AutenticacaoResponse> Autenticar(AutenticacaoRequest autenticacaoRequest)
        {
            var autenticacaoDto = await _autenticacaoUseCases.AutenticarUseCaseAsync(
                _autenticacaoPresenter.ParaUseCaseDto(autenticacaoRequest));
                
            return new AutenticacaoResponse
            {
                Token = autenticacaoDto.Token
            };
        }
    }
}
