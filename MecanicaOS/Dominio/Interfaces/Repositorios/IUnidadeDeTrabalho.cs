namespace Dominio.Interfaces.Repositorios
{
    public interface IUnidadeDeTrabalho
    {
        Task<bool> Commit();
    }
}
