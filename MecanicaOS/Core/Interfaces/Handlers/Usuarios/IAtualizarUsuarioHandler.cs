using Core.DTOs.UseCases.Usuario;
using Core.UseCases.Usuarios.AtualizarUsuario;

namespace Core.Interfaces.Handlers.Usuarios
{
    /// <summary>
    /// Interface para o handler de atualização de usuários
    /// </summary>
    public interface IAtualizarUsuarioHandler
    {
        /// <summary>
        /// Manipula a operação de atualização de usuário
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado</param>
        /// <param name="request">DTO com os dados atualizados do usuário</param>
        /// <returns>Resposta contendo o usuário atualizado</returns>
        Task<AtualizarUsuarioResponse> Handle(Guid id, AtualizarUsuarioUseCaseDto request);
    }
}
