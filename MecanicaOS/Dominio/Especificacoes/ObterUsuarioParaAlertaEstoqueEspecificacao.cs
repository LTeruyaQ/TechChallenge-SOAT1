using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterUsuarioParaAlertaEstoqueEspecificacao : IEspecificacao<Usuario>
{
    public Expression<Func<Usuario, bool>> Expressao =>
     u => u.RecebeAlertaEstoque;
}