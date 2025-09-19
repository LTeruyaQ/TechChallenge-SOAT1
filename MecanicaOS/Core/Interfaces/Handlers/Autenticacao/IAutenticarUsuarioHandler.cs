using Core.DTOs.UseCases.Autenticacao;
using Core.UseCases.Autenticacao.AutenticarUsuario;

namespace Core.Interfaces.Handlers.Autenticacao
{
    /// <summary>
    /// Interface para o handler de autenticação de usuário
    /// </summary>
    public interface IAutenticarUsuarioHandler
    {
        /// <summary>
        /// Manipula a operação de autenticação de usuário
        /// </summary>
        /// <param name="request">DTO com os dados de autenticação</param>
        /// <returns>Resposta contendo os dados de autenticação e token</returns>
        Task<AutenticarUsuarioResponse> Handle(AutenticacaoUseCaseDto request);
    }
}
