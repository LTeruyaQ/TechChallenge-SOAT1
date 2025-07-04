﻿using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public class OuEspecificacao<T> : IEspecificacao<T>
    {
        public IEspecificacao<T> Esquerda { get; }
        public IEspecificacao<T> Direita { get; }

        public OuEspecificacao(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            Esquerda = esquerda;
            Direita = direita;
        }

        public Expression<Func<T, bool>> Expressao
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