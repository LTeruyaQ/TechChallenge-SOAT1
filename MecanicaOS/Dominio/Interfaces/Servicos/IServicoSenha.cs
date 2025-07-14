namespace Dominio.Interfaces.Servicos
{
    public interface IServicoSenha
    {
        string CriptografarSenha(string senha);
        bool VerificarSenha(string senha, string hashSenha);
    }
}
