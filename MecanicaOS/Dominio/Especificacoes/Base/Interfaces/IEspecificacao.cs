using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base.Interfaces
{
    public interface IEspecificacao<T>
    {
        Expression<Func<T, bool>> Expressao { get; }
        List<Expression<Func<T, object>>> Inclusoes { get; }
        List<string> InclusoesPorString { get; }

        IEspecificacao<T> Incluir(Expression<Func<T, object>> expressaoInclusao);
        IEspecificacao<T> Incluir(string stringInclusao);
    }
}
