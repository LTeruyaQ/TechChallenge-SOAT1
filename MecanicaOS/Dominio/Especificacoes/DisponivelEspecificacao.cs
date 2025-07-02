using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ServicoDisponivelEspecificacao : IEspecificacao<Servico>
    {
        public Expression<Func<Servico, bool>> Expressao => s => s.Disponivel;
    }
}
