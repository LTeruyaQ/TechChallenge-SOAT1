using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public abstract class EspecificacaoBase<T> : IEspecificacao<T>
    {
        public virtual Expression<Func<T, bool>> Expressao { get; }

        public virtual List<Func<IQueryable<T>, IQueryable<T>>> Inclusoes { get; } = new();

        protected void AdicionarInclusao(Func<IQueryable<T>, IQueryable<T>> includeExpression)
        {
            Inclusoes.Add(includeExpression);
        }
    }
}