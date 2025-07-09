using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public class EEspecificacaoComInclusoes<T> : EEspecificacao<T>
    {
        public EEspecificacaoComInclusoes(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
            : base(esquerda, direita)
        {
            if (esquerda == null) throw new ArgumentNullException(nameof(esquerda));
            if (direita == null) throw new ArgumentNullException(nameof(direita));
        }

        public override Expression<Func<T, bool>> Expressao
        {
            get
            {
                var parametro = Expression.Parameter(typeof(T));
                var esquerda = base._esquerda.Expressao;
                var direita = base._direita.Expressao;

                var esquerdaBody = Expression.Invoke(esquerda, parametro);
                var direitaBody = Expression.Invoke(direita, parametro);
                var corpo = Expression.AndAlso(esquerdaBody, direitaBody);

                return Expression.Lambda<Func<T, bool>>(corpo, parametro);
            }
        }
    }
}
