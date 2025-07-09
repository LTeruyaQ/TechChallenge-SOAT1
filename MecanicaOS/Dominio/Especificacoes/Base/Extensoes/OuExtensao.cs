using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes.Base.Extensoes
{
    public static class OuExtensao
    {
        public static IEspecificacao<T> Ou<T>(this IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            if (esquerda == null) throw new ArgumentNullException(nameof(esquerda));
            if (direita == null) throw new ArgumentNullException(nameof(direita));

            return new OuEspecificacaoComInclusoes<T>(esquerda, direita);
        }
    }
}
