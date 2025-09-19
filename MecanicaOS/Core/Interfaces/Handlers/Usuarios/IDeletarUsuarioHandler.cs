using Core.UseCases.Usuarios.DeletarUsuario;

namespace Core.Interfaces.Handlers.Usuarios
{
    /// <summary>
    /// Interface para o handler de deleção de usuários
    /// </summary>
    public interface IDeletarUsuarioHandler
    {
        /// <summary>
        /// Manipula a operação de deleção de usuário
        /// </summary>
        /// <param name="id">ID do usuário a ser deletado</param>
        /// <returns>Resposta indicando o sucesso da operação</returns>
        Task<DeletarUsuarioResponse> Handle(Guid id);
    }
}
