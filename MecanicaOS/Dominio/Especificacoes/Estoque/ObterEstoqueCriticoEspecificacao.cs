using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Estoque;

public class ObterEstoqueCriticoEspecificacao : EspecificacaoBase<Entidades.Estoque>
{
    public override Expression<Func<Entidades.Estoque, bool>> Expressao =>
         e => e.QuantidadeDisponivel <= e.QuantidadeMinima;
}