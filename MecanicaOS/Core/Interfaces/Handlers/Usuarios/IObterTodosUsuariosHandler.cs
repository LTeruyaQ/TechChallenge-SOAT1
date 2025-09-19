using Core.UseCases.Usuarios.ObterTodosUsuarios;

namespace Core.Interfaces.Handlers.Usuarios
{
    /// <summary>
    /// Interface para o handler de obtenção de todos os usuários
    /// </summary>
    public interface IObterTodosUsuariosHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de todos os usuários
        /// </summary>
        /// <returns>Resposta contendo a lista de usuários</returns>
        Task<ObterTodosUsuariosResponse> Handle();
    }
}
