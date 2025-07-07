using System.Linq.Expressions;
using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes;

public class EstoqueCriticoEspecificacao : IEspecificacao<Estoque>
{
    public Expression<Func<Estoque, bool>> Expressao =>
         e => e.QuantidadeDisponivel <= e.QuantidadeMinima;
}