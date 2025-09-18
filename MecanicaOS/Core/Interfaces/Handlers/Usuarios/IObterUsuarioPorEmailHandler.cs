using Core.UseCases.Usuarios.ObterUsuarioPorEmail;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Usuarios
{
    /// <summary>
    /// Interface para o handler de obtenção de usuário por email
    /// </summary>
    public interface IObterUsuarioPorEmailHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de usuário por email
        /// </summary>
        /// <param name="email">Email do usuário a ser obtido</param>
        /// <returns>Resposta contendo o usuário encontrado</returns>
        Task<ObterUsuarioPorEmailResponse> Handle(string email);
    }
}
