namespace Dominio.Exceptions;

public class RegistroNaoEncontradaException : Exception
{
    public RegistroNaoEncontradaException(string mensagem) : base(mensagem) { }
}

