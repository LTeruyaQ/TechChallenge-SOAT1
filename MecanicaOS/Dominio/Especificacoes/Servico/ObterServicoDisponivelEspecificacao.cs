using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Servico
{
    public class ObterServicoDisponivelEspecificacao : EspecificacaoBase<Entidades.Servico>
    {
        public override Expression<Func<Entidades.Servico, bool>> Expressao => s => s.Disponivel;
    }
}
