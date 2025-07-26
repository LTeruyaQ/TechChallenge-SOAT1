using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Usuario;

public class ObterUsuarioParaAlertaEstoqueEspecificacao : EspecificacaoBase<Entidades.Usuario>
{
    public override Expression<Func<Entidades.Usuario, bool>> Expressao =>
     u => u.RecebeAlertaEstoque;
}