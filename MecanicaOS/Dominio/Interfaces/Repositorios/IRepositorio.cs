using Dominio.Entidades;
using Dominio.Entidades.Abstratos;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Especificacoes.Cliente;

namespace Dominio.Interfaces.Repositorios;

public interface IRepositorio<T> where T : Entidade
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<T?> ObterUmPorFiltroAsync(IEspecificacao<T> especificacao);
    Task<IEnumerable<T>> ObterPorFiltroAsync(IEspecificacao<T> filtro);
    Task<IEnumerable<T>> ObterPorFiltroSemRastreamentoAsync(IEspecificacao<T> especificacao);
    Task<IEnumerable<T>> ObterPorFiltroPaginadoSemRastreamentoAsync(IEspecificacao<T> especificacao);
    Task<IEnumerable<T>> ObterPorFiltroPaginadoAsync(IEspecificacao<T> especificacao);
    Task<T> CadastrarAsync(T entidade);
    Task<IEnumerable<T>> CadastrarVariosAsync(IEnumerable<T> entidades);
    Task EditarAsync(T novaEntidade);
    Task EditarVariosAsync(IEnumerable<T> entidades);
    Task DeletarAsync(T entidade);
    Task DeletarVariosAsync(IEnumerable<T> entidades);
    Task<T?> ObterUmSemRastreamentoAsync(IEspecificacao<T> especificacao);
    Task<T?> ObterUmAsync(IEspecificacao<T> especificacao);
    Task DeletarLogicamenteAsync(T entidade);

    // Novos métodos para projeção
    Task<TProjecao?> ObterProjetadoAsync<TProjecao>(
        IEspecificacao<T> especificacao,
        CancellationToken cancellationToken = default);

    Task<TProjecao?> ObterProjetadoSemRastreamentoAsync<TProjecao>(
        IEspecificacao<T> especificacao,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TProjecao>> ListarProjetadoComPaginacaoAsync<TProjecao>(
        IEspecificacao<T> especificacao,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TProjecao>> ListarProjetadoComPaginacaoSemRastreamentoAsync<TProjecao>(
        IEspecificacao<T> especificacao,
        CancellationToken cancellationToken = default);
}