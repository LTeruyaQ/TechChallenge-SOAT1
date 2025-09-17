using Core.DTOs.UseCases.Usuario;

namespace Core.UseCases.Usuarios.CadastrarUsuario
{
    public class CadastrarUsuarioCommand
    {
        public CadastrarUsuarioUseCaseDto Request { get; set; }

        public CadastrarUsuarioCommand(CadastrarUsuarioUseCaseDto request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
