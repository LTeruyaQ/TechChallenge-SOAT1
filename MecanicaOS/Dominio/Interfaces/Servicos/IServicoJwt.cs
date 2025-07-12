namespace Dominio.Interfaces.Servicos
{
    public interface IServicoJwt
    {
        string GerarToken(Guid usuarioId, string email, string tipoUsuario);
        bool ValidarToken(string token);
    }
}
