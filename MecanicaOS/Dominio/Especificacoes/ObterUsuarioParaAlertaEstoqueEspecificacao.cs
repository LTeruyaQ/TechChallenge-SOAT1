using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterUsuarioParaAlertaEstoqueEspecificacao : EspecificacaoBase<Usuario>
{
    public override Expression<Func<Usuario, bool>> Expressao =>
     u => u.RecebeAlertaEstoque;
}