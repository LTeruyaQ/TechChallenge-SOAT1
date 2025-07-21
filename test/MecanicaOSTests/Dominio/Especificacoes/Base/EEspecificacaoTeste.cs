using Dominio.Especificacoes.Base;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace MecanicaOSTests.Dominio.Especificacoes.Base
{
    public class EEspecificacaoTeste<T> : EEspecificacao<T> where T : class
    {
        public EEspecificacaoTeste(IEspecificacao<T> esquerda, IEspecificacao<T> direita)
            : base(esquerda, direita)
        {
        }

        // Propriedade para expor as inclusões para fins de teste
        public IReadOnlyCollection<string> InclusoesPublicas => base.Inclusoes;

        // Métodos públicos para adicionar inclusões que delegam para os métodos protegidos da classe base
        public new void AdicionarInclusao<TProp>(Expression<Func<T, TProp>> navegacao)
        {
            base.AdicionarInclusao(navegacao);
        }

        public new void AdicionarInclusao<TColecao, TProp>(
            Expression<Func<T, IEnumerable<TColecao>>> colecao,
            Expression<Func<TColecao, TProp>> navegacao)
        {
            base.AdicionarInclusao(colecao, navegacao);
        }
    }
}
