using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterClientePorDocumento : EspecificacaoBase<Cliente>
    {
        private string documento;

        public ObterClientePorDocumento(string documento)
        {
            this.documento = documento;
        }

        public override Expression<Func<Cliente, bool>> Expressao => c => c.Documento == documento;
    }
}