using Dominio.Especificacoes.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infraestrutura.Dados.Especificacoes
{
    public static class AvaliadorDeEspecificacao<T> where T : class
    {
        public static IQueryable<T> ObterConsulta(
            IQueryable<T> consultaInicial,
            IEspecificacao<T> especificacao)
        {
            var consulta = consultaInicial;

            if (especificacao.Expressao != null)
            {
                consulta = consulta.Where(especificacao.Expressao);
            }

            if (especificacao.Inclusoes != null && especificacao.Inclusoes.Any())
            {
                consulta = especificacao.Inclusoes
                    .Aggregate(consulta, (current, include) => current.Include(include));
            }

            return consulta;
        }

        public static IQueryable<TProjecao> AplicarProjecao<TProjecao>(
            IQueryable<T> consulta,
            IEspecificacao<T> especificacao)
        {
            if (!especificacao.UsarProjecao)
            {
                throw new InvalidOperationException("A especificaÃ§Ã£o nÃ£o contÃ©m uma projeÃ§Ã£o definida.");
            }

            var projecao = especificacao.ObterProjecao() as Expression<Func<T, TProjecao>>;
            if (projecao == null)
            {
                var projecaoObj = especificacao.ObterProjecao() as Expression<Func<T, object>>;
                if (projecaoObj == null)
                {
                    throw new InvalidOperationException("Tipo de projeÃ§Ã£o incompatÃ­vel.");
                }

                var parametro = Expression.Parameter(typeof(T), "x");
                var corpo = Expression.Convert(
                    Expression.Invoke(projecaoObj, parametro),
                    typeof(TProjecao)
                );
                projecao = Expression.Lambda<Func<T, TProjecao>>(corpo, parametro);
            }

            return consulta.Select(projecao);
        }

        public static IQueryable<T> ObterConsultaSemRastreamento(
            IQueryable<T> consultaInicial,
            IEspecificacao<T> especificacao)
        {
            return ObterConsulta(consultaInicial, especificacao).AsNoTracking();
        }
    }
}
