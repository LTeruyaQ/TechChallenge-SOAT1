namespace Dominio.Exceptions
{
    [Serializable]
    public class RegistroJaCadastradoException : Exception
    {
        public RegistroJaCadastradoException()
        {
        }

        public RegistroJaCadastradoException(string? message) : base(message)
        {
        }

        public RegistroJaCadastradoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}