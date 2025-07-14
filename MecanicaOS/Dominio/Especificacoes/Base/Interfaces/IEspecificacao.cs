using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base.Interfaces
{
    public interface IEspecificacao<T>
    {
        Expression<Func<T, bool>> Expressao { get; }

        List<Expression<Func<T, object>>> Inclusoes { get; }
    }
}
