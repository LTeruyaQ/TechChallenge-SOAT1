using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Servico
{
    public class ObterServicoDisponivelEspecificacao : EspecificacaoBase<Entidades.Servico>
    {
        public override Expression<Func<Entidades.Servico, bool>> Expressao => s => s.Disponivel;
    }
}
