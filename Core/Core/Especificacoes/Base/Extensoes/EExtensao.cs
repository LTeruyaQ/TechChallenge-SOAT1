using Core.Especificacoes.Base.Interfaces;

namespace Core.Especificacoes.Base.Extensoes
{
    public static class EExtensao
    {
        public static IEspecificacao<T> E<T>(this IEspecificacao<T> esquerda, IEspecificacao<T> direita) where T : class
        {
            return new EEspecificacao<T>(esquerda, direita);
        }
    }
}
