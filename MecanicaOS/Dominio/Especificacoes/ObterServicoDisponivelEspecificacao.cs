using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterServicoDisponivelEspecificacao : Especificacao<Servico>
    {
        public override Expression<Func<Servico, bool>> Expressao => s => s.Disponivel;
    }
}
