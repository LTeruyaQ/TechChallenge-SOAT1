using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Usuario;

public class ObterUsuarioParaAlertaEstoqueEspecificacao : EspecificacaoBase<Entidades.Usuario>
{
    public override Expression<Func<Entidades.Usuario, bool>> Expressao =>
     u => u.RecebeAlertaEstoque;
}