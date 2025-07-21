using Dominio.Especificacoes.Base.Interfaces;
using System;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public class OuEspecificacao<T> : EspecificacaoBase<T> where T : class
    {
        public IEspecificacao<T> Esquerda { get; }
        public IEspecificacao<T> Direita { get; }

        public OuEspecificacao(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            Esquerda = esquerda ?? throw new ArgumentNullException(nameof(esquerda));
            Direita = direita ?? throw new ArgumentNullException(nameof(direita));

            // Combina as inclusões das especificações filhas
            if (esquerda.Inclusoes != null)
                _ = esquerda.Inclusoes.Select(Inclusoes.Add);

            if (direita.Inclusoes != null)
                _ = direita.Inclusoes.Select(Inclusoes.Add);
        }

        public override Expression<Func<T, bool>> Expressao
        {
            get
            {
                var param = Expression.Parameter(typeof(T), "x");

                var esquerdaBody = Expression.Invoke(Esquerda.Expressao, param);
                var direitaBody = Expression.Invoke(Direita.Expressao, param);

                var body = Expression.OrElse(esquerdaBody, direitaBody);

                return Expression.Lambda<Func<T, bool>>(body, param);
            }
        }
    }
}