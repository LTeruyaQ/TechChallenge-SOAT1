using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes.Base.Extensoes
{
    public static class OuExtensao
    {
        public static void Ou<T>(this IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            esquerda = new OuEspecificacao<T>(esquerda, direita);
        }
    }
}
