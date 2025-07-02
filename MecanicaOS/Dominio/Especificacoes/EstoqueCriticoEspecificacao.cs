using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class EstoqueCriticoEspecificacao : IEspecificacao<Estoque>
{
    public Expression<Func<Estoque, bool>> Expressao =>
         e => e.QuantidadeDisponivel <= e.QuantidadeMinima;
}