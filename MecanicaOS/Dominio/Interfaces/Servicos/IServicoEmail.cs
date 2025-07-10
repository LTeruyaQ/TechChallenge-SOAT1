namespace Dominio.Interfaces.Servicos;

public interface IServicoEmail
{
    Task EnviarAsync(IEnumerable<string> emailsDestino, string assunto, string conteudo);
}