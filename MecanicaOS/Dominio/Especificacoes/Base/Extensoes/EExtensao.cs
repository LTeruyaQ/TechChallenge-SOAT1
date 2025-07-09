using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes.Base.Extensoes
{
    public static class EExtensao
    {
        public static IEspecificacao<T> E<T>(this IEspecificacao<T> esquerda, IEspecificacao<T> direita)
        {
            if (esquerda == null) throw new ArgumentNullException(nameof(esquerda));
            if (direita == null) throw new ArgumentNullException(nameof(direita));

            return new EEspecificacaoComInclusoes<T>(esquerda, direita);
        }
    }
}
