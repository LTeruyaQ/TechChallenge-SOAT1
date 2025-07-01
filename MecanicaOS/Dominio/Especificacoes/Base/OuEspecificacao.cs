using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes.Base
{
    public class OuEspecificacao<T> : IEspecificacao<T>
    {
        public IEspecificacao<T> Esquerda { get; }
        public IEspecificacao<T> Direita { get; }
        public OuEspecificacao(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            Esquerda = esquerda;
            Direita = direita;
        }
        public bool EhSatisfeitoPor(T item) => Esquerda.EhSatisfeitoPor(item) || Direita.EhSatisfeitoPor(item);
    }
}
