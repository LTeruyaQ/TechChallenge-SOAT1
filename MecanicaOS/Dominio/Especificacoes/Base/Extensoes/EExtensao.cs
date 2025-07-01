using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes.Base.Extensoes
{
    public static class EExtensao
    {
        public static IEspecificacao<T> E<T>(this IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            return new EEspecificacao<T>(esquerda, direita);
        }
    }
}
