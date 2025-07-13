using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public class EEspecificacao<T> : EspecificacaoBase<T>
    {
        public IEspecificacao<T> Esquerda { get; }
        public IEspecificacao<T> Direita { get; }

        public EEspecificacao(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            Esquerda = esquerda;
            Direita = direita;

            Inclusoes.AddRange(esquerda.Inclusoes);
            Inclusoes.AddRange(direita.Inclusoes);
        }

        public override Expression<Func<T, bool>> Expressao
        {
            get
            {
                var param = Expression.Parameter(typeof(T));

                var esquerdaBody = Expression.Invoke(Esquerda.Expressao, param);
                var direitaBody = Expression.Invoke(Direita.Expressao, param);

                var body = Expression.AndAlso(esquerdaBody, direitaBody);

                return Expression.Lambda<Func<T, bool>>(body, param);
            }
        }
    }

}