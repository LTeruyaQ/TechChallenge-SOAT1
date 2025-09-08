using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Estoque;

public class ObterEstoqueCriticoEspecificacao : EspecificacaoBase<Entidades.Estoque>
{
    public override Expression<Func<Entidades.Estoque, bool>> Expressao =>
         e => e.QuantidadeDisponivel <= e.QuantidadeMinima;
}