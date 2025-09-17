namespace Core.UseCases.Usuarios.ObterUsuario
{
    public class ObterUsuarioUseCase
    {
        public Guid Id { get; set; }

        public ObterUsuarioUseCase(Guid id)
        {
            Id = id;
        }
    }
}
