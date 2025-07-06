namespace Dominio.Exceptions
{
    [Serializable]
    public class DadosJaCadastradoException : Exception
    {
        public DadosJaCadastradoException()
        {
        }

        public DadosJaCadastradoException(string? message) : base(message)
        {
        }

        public DadosJaCadastradoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}