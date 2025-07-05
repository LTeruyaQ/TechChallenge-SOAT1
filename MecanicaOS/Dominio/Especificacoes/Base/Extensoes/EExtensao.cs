using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes.Base.Extensoes
{
    public static class EExtensao
    {
        public static void E<T>(this IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            esquerda = new EEspecificacao<T>(esquerda, direita);
        }
    }
}
