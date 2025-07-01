namespace Dominio.Especificacoes.Base.Interfaces
{
    public interface IEspecificacao<T>
    {
        bool EhSatisfeitoPor(T item);
    }
}
