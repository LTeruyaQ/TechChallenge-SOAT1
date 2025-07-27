using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Usuario;

public class ObterUsuarioPorEmailEspecificacao : EspecificacaoBase<Entidades.Usuario>
{
    private string email;

    public ObterUsuarioPorEmailEspecificacao(string email)
    {
        this.email = email;
    }

    public override Expression<Func<Entidades.Usuario, bool>> Expressao => u => u.Email == email;
}
