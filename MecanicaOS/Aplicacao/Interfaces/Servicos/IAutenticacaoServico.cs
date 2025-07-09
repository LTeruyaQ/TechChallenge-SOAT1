using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Responses.Autenticacao;

namespace Aplicacao.Interfaces.Servicos
{
    public interface IAutenticacaoServico
    {
        Task<LoginResponse> AutenticarAsync(LoginRequest request);
        Task<LoginResponse> RegistrarAsync(RegistroRequest request);
        Task<bool> ValidarCredenciaisAsync(string login, string senha);
    }
}
