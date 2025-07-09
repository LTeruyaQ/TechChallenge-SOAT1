using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public abstract class Especificacao<T> : IEspecificacao<T>
    {
        protected List<Expression<Func<T, object>>> _inclusoes = new();
        protected List<string> _inclusoesPorString = new();

        public List<Expression<Func<T, object>>> Inclusoes => _inclusoes;
        public List<string> InclusoesPorString => _inclusoesPorString;

        public abstract Expression<Func<T, bool>> Expressao { get; }

        public IEspecificacao<T> Incluir(Expression<Func<T, object>> expressaoInclusao)
        {
            _inclusoes.Add(expressaoInclusao);
            return this;
        }

        public IEspecificacao<T> Incluir(string stringInclusao)
        {
            if (!string.IsNullOrWhiteSpace(stringInclusao))
            {
                _inclusoesPorString.Add(stringInclusao);
            }
            return this;
        }
    }
}
