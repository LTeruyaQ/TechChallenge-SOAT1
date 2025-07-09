using Dominio.Especificacoes.Base.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public abstract class OuEspecificacaoBase<T> : IEspecificacao<T>
    {
        private readonly Lazy<List<Expression<Func<T, object>>>> _inclusoes;
        private readonly Lazy<List<string>> _inclusoesPorString;
        protected readonly IEspecificacao<T> _esquerda;
        protected readonly IEspecificacao<T> _direita;
        private static readonly ConcurrentDictionary<string, int> _cacheHash = new();

        protected OuEspecificacaoBase(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            _esquerda = esquerda ?? throw new ArgumentNullException(nameof(esquerda));
            _direita = direita ?? throw new ArgumentNullException(nameof(direita));

            _inclusoes = new Lazy<List<Expression<Func<T, object>>>>(() =>
            {
                var lista = new List<Expression<Func<T, object>>>();

                if (_esquerda?.Inclusoes != null)
                    lista.AddRange(_esquerda.Inclusoes);

                if (_direita?.Inclusoes != null)
                    lista.AddRange(_direita.Inclusoes);

                return lista.Distinct(new ExpressionComparer()).ToList();
            });

            _inclusoesPorString = new Lazy<List<string>>(() =>
            {
                var lista = new List<string>();

                if (_esquerda?.InclusoesPorString != null)
                    lista.AddRange(_esquerda.InclusoesPorString);

                if (_direita?.InclusoesPorString != null)
                    lista.AddRange(_direita.InclusoesPorString);

                return lista.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            });
        }

        public List<Expression<Func<T, object>>> Inclusoes => _inclusoes.Value;
        public List<string> InclusoesPorString => _inclusoesPorString.Value;

        public abstract Expression<Func<T, bool>> Expressao { get; }

        public IEspecificacao<T> Incluir(Expression<Func<T, object>> expressaoInclusao)
        {
            throw new NotSupportedException("Não é possível adicionar inclusões a uma especificação combinada. Adicione as inclusões antes de combinar.");
        }

        public IEspecificacao<T> Incluir(string stringInclusao)
        {
            throw new NotSupportedException("Não é possível adicionar inclusões a uma especificação combinada. Adicione as inclusões antes de combinar.");
        }

        private class ExpressionComparer : IEqualityComparer<Expression<Func<T, object>>>
        {
            public bool Equals(Expression<Func<T, object>> x, Expression<Func<T, object>> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;

                return x.ToString() == y.ToString();
            }

            public int GetHashCode(Expression<Func<T, object>> obj)
            {
                return obj?.ToString().GetHashCode() ?? 0;
            }
        }
    }
}
