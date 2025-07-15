using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Responses.Autenticacao;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IAutenticacaoServico
    {
        Task<AutenticacaoResponse> AutenticarAsync(AutenticacaoRequest request);
    }
}
