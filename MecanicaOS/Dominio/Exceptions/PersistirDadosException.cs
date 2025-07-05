namespace Dominio.Exceptions;

public class PersistirDadosException : Exception
{
    public PersistirDadosException(string mensagem) : base(mensagem) { }
}

