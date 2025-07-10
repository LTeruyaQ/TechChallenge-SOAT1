using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterUsuarioPorEmailEspecificacao : IEspecificacao<Usuario>
{
    private string email;

    public ObterUsuarioPorEmailEspecificacao(string email)
    {
        this.email = email;
    }

    public Expression<Func<Usuario, bool>> Expressao => u => u.Email == email;
}
