namespace Core.UseCases.Usuarios.DeletarUsuario
{
    public class DeletarUsuarioCommand
    {
        public Guid Id { get; set; }

        public DeletarUsuarioCommand(Guid id)
        {
            Id = id;
        }
    }
}
