namespace Core.UseCases.Usuarios.ObterUsuarioPorEmail
{
    public class ObterUsuarioPorEmailUseCase
    {
        public string Email { get; set; }

        public ObterUsuarioPorEmailUseCase(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    }
}
