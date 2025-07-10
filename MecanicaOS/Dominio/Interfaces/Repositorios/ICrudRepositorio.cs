using Dominio.Entidades.Abstratos;
using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Interfaces.Repositorios;

public interface IRepositorio<T> where T : Entidade
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<IEnumerable<T>> ObterPorFiltroAsync(IEspecificacao<T> filtro);
    Task<T> CadastrarAsync(T entidade);
    Task<IEnumerable<T>> CadastrarVariosAsync(IEnumerable<T> entidades);
    Task EditarAsync(T novaEntidade);
    Task DeletarAsync(T entidade);
    Task<T?> ObterUmAsync(IEspecificacao<T> especificacao);
    Task DeletarLogicamenteAsync(T entidade); 
}