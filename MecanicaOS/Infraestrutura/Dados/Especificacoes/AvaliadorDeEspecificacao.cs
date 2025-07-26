using Dominio.Especificacoes.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Dados.Especificacoes;

public static class AvaliadorDeEspecificacao<T> where T : class
{
    public static IQueryable<T> ObterConsulta(IQueryable<T> consultaInicial, IEspecificacao<T> especificacao)
    {
        var consulta = consultaInicial;

        consulta = AplicarInclusoes(especificacao, consulta);
        consulta = AplicarFiltros(especificacao, consulta);

        return consulta;
    }

    public static IQueryable<T> ObterConsultaSemRastreanemento(IQueryable<T> consultaInicial, IEspecificacao<T> especificacao)
    {
        var consulta = consultaInicial.AsNoTracking();
        
        consulta = AplicarInclusoes(especificacao, consulta);
        consulta = AplicarFiltros(especificacao, consulta);

        return consulta;
    }

    private static IQueryable<T> AplicarFiltros(IEspecificacao<T> especificacao, IQueryable<T> consulta)
    {
        if (especificacao.Expressao != null)
        {
            consulta = consulta.Where(especificacao.Expressao);
        }

        return consulta;
    }

    private static IQueryable<T> AplicarInclusoes(IEspecificacao<T> especificacao, IQueryable<T> consulta)
    {
        if (especificacao.Inclusoes != null && especificacao.Inclusoes.Any())
        {
            foreach (var includeExpression in especificacao.Inclusoes)
            {
                consulta = consulta.Include(includeExpression);
            }
        }

        return consulta;
    }
}
