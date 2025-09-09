using Adapters.DTOs.Requests.Autenticacao;
using Adapters.DTOs.Responses.Autenticacao;

namespace Adapters.Controllers
{
    public interface IAutenticacaoController
    {
        Task<AutenticacaoResponse> AutenticarAsync(AutenticacaoRequest autenticacaoRequest);
    }
}