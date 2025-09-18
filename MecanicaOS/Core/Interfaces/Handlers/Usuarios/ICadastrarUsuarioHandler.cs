using Core.DTOs.UseCases.Usuario;
using Core.UseCases.Usuarios.CadastrarUsuario;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Usuarios
{
    /// <summary>
    /// Interface para o handler de cadastro de usuários
    /// </summary>
    public interface ICadastrarUsuarioHandler
    {
        /// <summary>
        /// Manipula a operação de cadastro de usuário
        /// </summary>
        /// <param name="request">DTO com os dados do usuário a ser cadastrado</param>
        /// <returns>Resposta contendo o usuário cadastrado</returns>
        Task<CadastrarUsuarioResponse> Handle(CadastrarUsuarioUseCaseDto request);
    }
}
