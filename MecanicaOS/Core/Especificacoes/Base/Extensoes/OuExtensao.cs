using Core.Especificacoes.Base.Interfaces;

namespace Core.Especificacoes.Base.Extensoes
{
    public static class OuExtensao
    {
        public static IEspecificacao<T> Ou<T>(this IEspecificacao<T> esquerda, IEspecificacao<T> direita) where T : class
        {
            return new OuEspecificacao<T>(esquerda, direita);
        }
    }
}
