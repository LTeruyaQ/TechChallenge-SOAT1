using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterEstoqueCriticoEspecificacao : Especificacao<Estoque>
{
    public override Expression<Func<Estoque, bool>> Expressao => e => e.QuantidadeDisponivel <= e.QuantidadeMinima;
}