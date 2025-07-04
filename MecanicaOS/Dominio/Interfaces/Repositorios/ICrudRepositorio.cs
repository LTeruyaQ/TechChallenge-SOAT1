using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Interfaces.Repositorios;

public interface ICrudRepositorio<T>
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<IEnumerable<T>> ObterPorFiltroAsync(IEspecificacao<T> filtro);
    Task<T> CadastrarAsync(T entidade);
    Task EditarAsync(T novaEntidade);
    Task DeletarAsync(T entidade);
}