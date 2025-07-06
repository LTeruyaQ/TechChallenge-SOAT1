namespace Dominio.Exceptions;

public class DadosNaoEncontradoException : Exception
{
    public DadosNaoEncontradoException(string mensagem) : base(mensagem) { }
}

