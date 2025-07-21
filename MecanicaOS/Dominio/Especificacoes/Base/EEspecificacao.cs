using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public class EEspecificacao<T> : EspecificacaoBase<T> where T : class
    {
        public IEspecificacao<T> Esquerda { get; }
        public IEspecificacao<T> Direita { get; }

        public EEspecificacao(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            Esquerda = esquerda ?? throw new ArgumentNullException(nameof(esquerda));
            Direita = direita ?? throw new ArgumentNullException(nameof(direita));

            if (esquerda.Inclusoes != null)
            {
                foreach (var inclusao in esquerda.Inclusoes)
                {
                    if (!Inclusoes.Contains(inclusao))
                    {
                        Inclusoes.Add(inclusao);
                    }
                }
            }

            if (direita.Inclusoes != null)
            {
                foreach (var inclusao in direita.Inclusoes)
                {
                    if (!Inclusoes.Contains(inclusao))
                    {
                        Inclusoes.Add(inclusao);
                    }
                }
            }
        }

        public override Expression<Func<T, bool>> Expressao
        {
            get
            {
                var param = Expression.Parameter(typeof(T), "x");

                var esquerdaBody = Expression.Invoke(Esquerda.Expressao, param);
                var direitaBody = Expression.Invoke(Direita.Expressao, param);

                var body = Expression.AndAlso(esquerdaBody, direitaBody);

                return Expression.Lambda<Func<T, bool>>(body, param);
            }
        }
    }
}