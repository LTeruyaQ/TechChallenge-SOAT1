namespace Dominio.Exceptions;

public class AutenticacaoException : Exception
{
    public AutenticacaoException(string message) : base(message) { }
}

public class CredenciaisInvalidasException : AutenticacaoException
{
    public CredenciaisInvalidasException() : base("Credenciais inválidas") { }
}

public class UsuarioNaoEncontradoException : AutenticacaoException
{
    public UsuarioNaoEncontradoException(string login)
        : base($"Usuário '{login}' não encontrado") { }
}

public class UsuarioJaCadastradoException : AutenticacaoException
{
    public UsuarioJaCadastradoException(string login)
        : base($"O login '{login}' já está em uso") { }
}

public class ClienteJaPossuiCadastroException : AutenticacaoException
{
    public ClienteJaPossuiCadastroException(string documento)
        : base($"Já existe um cadastro ativo para o documento {documento}") { }
}