using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class UsuarioPorLoginEspecificacao : Especificacao<Usuario>
{
    private readonly string _login;

    public UsuarioPorLoginEspecificacao(string login)
    {
        _login = login;
    }

    public override Expression<Func<Usuario, bool>> Expressao =>
        usuario => usuario.Login == _login;
}
