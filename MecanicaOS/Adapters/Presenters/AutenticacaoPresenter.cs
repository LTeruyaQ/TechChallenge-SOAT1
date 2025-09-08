using Adapters.DTOs.Requests.Autenticacao;
using Adapters.DTOs.Responses.Autenticacao;
using Adapters.Presenters.Interfaces;
using Core.DTOs.Autenticacao;

namespace Adapters.Presenters
{

    public class AutenticacaoPresenter : IAutenticacaoPresenter
    {
        public AutenticacaoUseCaseDto ParaUseCaseDto(AutenticacaoRequest request)
        {
            if (request == null)
                return null;

            return new AutenticacaoUseCaseDto
            {
                Email = request.Email,
                Senha = request.Senha
            };
        }

        public AutenticacaoResponse ParaResponse(AutenticacaoDto autenticacaoUseCaseDto)
        {
            if (autenticacaoUseCaseDto == null)
                return null;
            return new AutenticacaoResponse()
            {
                Token = autenticacaoUseCaseDto.Token
            };
        }
    }
}
