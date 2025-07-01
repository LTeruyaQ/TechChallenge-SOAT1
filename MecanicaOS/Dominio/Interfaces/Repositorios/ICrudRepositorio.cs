using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Interfaces.Repositorios;

public interface ICrudRepositorio<T>
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<T>> ObterTodos();
    Task<IEnumerable<T>> ObterPorFiltro(IEspecificacao<T> filtro);
    Task<T> CadastrarAsync(T entidade);
    Task Editar(T novaEntidade);
    Task DeletarAsync(T entidade);
}