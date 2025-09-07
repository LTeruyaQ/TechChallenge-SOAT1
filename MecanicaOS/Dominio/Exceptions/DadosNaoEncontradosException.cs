namespace Dominio.Exceptions;

public class DadosNaoEncontradosException(string mensagem) : DomainException(mensagem)
{
}