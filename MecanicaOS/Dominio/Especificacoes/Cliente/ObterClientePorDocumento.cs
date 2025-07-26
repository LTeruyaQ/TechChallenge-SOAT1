using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Cliente
{
    public class ObterClientePorDocumento : EspecificacaoBase<Entidades.Cliente>
    {
        private string documento;

        public ObterClientePorDocumento(string documento)
        {
            this.documento = documento;
        }

        public override Expression<Func<Entidades.Cliente, bool>> Expressao => c => c.Documento == documento;
    }
}