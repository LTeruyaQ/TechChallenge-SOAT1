using Core.Entidades;

namespace Core.UseCases.Usuarios.ObterTodosUsuarios
{
    public class ObterTodosUsuariosResponse
    {
        public IEnumerable<Usuario> Usuarios { get; set; } = Enumerable.Empty<Usuario>();
    }
}
