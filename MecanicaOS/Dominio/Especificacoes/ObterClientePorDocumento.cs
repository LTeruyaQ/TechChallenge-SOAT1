using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterClientePorDocumento : IEspecificacao<Cliente>
    {
        private string documento;

        public ObterClientePorDocumento(string documento)
        {
            this.documento = documento;
        }

        public Expression<Func<Cliente, bool>> Expressao => c => c.Documento == documento;
    }
}