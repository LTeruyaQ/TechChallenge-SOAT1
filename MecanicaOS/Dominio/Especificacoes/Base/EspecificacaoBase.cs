using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public abstract class EspecificacaoBase<T> : IEspecificacao<T>
    {
        public virtual Expression<Func<T, bool>> Expressao { get; }
        public virtual List<Expression<Func<T, object>>> Inclusoes { get; } = new();

        protected void AdicionarInclusao(Expression<Func<T, object>> includeExpression)
        {
            Inclusoes.Add(includeExpression);
        }
    }
}
