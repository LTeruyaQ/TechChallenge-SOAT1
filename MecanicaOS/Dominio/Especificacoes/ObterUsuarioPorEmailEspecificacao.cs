using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterUsuarioPorEmailEspecificacao : EspecificacaoBase<Usuario>
{
    private string email;

    public ObterUsuarioPorEmailEspecificacao(string email)
    {
        this.email = email;
    }

    public override Expression<Func<Usuario, bool>> Expressao => u => u.Email == email;
}
