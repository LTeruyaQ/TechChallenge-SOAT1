using Dominio.Especificacoes.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Dados.Especificacoes;

public static class AvaliadorDeEspecificacao<T> where T : class
{
    public static IQueryable<T> ObterConsulta(IQueryable<T> consultaInicial, IEspecificacao<T> especificacao)
    {
        var consulta = consultaInicial;

        if (especificacao.Inclusoes != null && especificacao.Inclusoes.Any())
        {
            foreach (var includeExpression in especificacao.Inclusoes)
            {
                consulta = consulta.Include(includeExpression);
            }
        }

        if (especificacao.Expressao != null)
        {
            consulta = consulta.Where(especificacao.Expressao);
        }

        return consulta;
    }

    public static IQueryable<T> ObterConsultaSemRastreanemento(IQueryable<T> consultaInicial, IEspecificacao<T> especificacao)
    {
        var consulta = consultaInicial.AsNoTracking();

        if (especificacao.Inclusoes != null && especificacao.Inclusoes.Any())
        {
            foreach (var includeExpression in especificacao.Inclusoes)
            {
                consulta = consulta.Include(includeExpression);
            }
        }

        if (especificacao.Expressao != null)
        {
            consulta = consulta.Where(especificacao.Expressao);
        }

        return consulta;
    }
}
