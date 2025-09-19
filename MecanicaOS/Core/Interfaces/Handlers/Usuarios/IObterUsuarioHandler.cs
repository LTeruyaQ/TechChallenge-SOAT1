using Core.UseCases.Usuarios.ObterUsuario;

namespace Core.Interfaces.Handlers.Usuarios
{
    /// <summary>
    /// Interface para o handler de obtenção de usuário por ID
    /// </summary>
    public interface IObterUsuarioHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de usuário por ID
        /// </summary>
        /// <param name="id">ID do usuário a ser obtido</param>
        /// <returns>Resposta contendo o usuário encontrado</returns>
        Task<ObterUsuarioResponse> Handle(Guid id);
    }
}
