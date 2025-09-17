using Core.DTOs.UseCases.Usuario;

namespace Core.UseCases.Usuarios.AtualizarUsuario
{
    public class AtualizarUsuarioCommand
    {
        public Guid Id { get; set; }
        public AtualizarUsuarioUseCaseDto Request { get; set; }

        public AtualizarUsuarioCommand(Guid id, AtualizarUsuarioUseCaseDto request)
        {
            Id = id;
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
