using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class UsuarioPorEmailEspecificacao : Especificacao<Usuario>
{
    private readonly string _login;

    public UsuarioPorEmailEspecificacao(string login)
    {
        _login = login;
    }

    public override Expression<Func<Usuario, bool>> Expressao =>
        usuario => usuario.Email == _login;
}
