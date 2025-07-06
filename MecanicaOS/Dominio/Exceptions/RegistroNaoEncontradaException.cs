namespace Dominio.Exceptions;

public class RegistroNaoEncontradoException : Exception
{
    public RegistroNaoEncontradoException(string mensagem) : base(mensagem) { }
}

