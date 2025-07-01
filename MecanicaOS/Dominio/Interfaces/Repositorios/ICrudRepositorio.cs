using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Interfaces.Repositorios
{
    public interface ICrudRepositorio<T>
    {
        Task<T> ObterPorId(Guid id);
        Task<IEnumerable<T>> ObterTodos();
        Task<IEnumerable<T>> ObterPorFiltro(IEspecificacao<T> filtro);
        Task Cadastrar(T entidade);
        Task Editar(Guid id, T novaEntidade);
        Task Delete(Guid id);
    }
}
