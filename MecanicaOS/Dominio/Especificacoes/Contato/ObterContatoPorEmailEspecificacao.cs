using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Contato
{
    public class ObterContatoPorEmailEspecificacao : EspecificacaoBase<Entidades.Contato>
    {
        private readonly string _email;
        private readonly Guid? _clienteId;

        public ObterContatoPorEmailEspecificacao(string email, Guid? clienteId = null)
        {
            _email = email;
            _clienteId = clienteId;
        }

        public override Expression<Func<Entidades.Contato, bool>> Expressao
        {
            get
            {
                return c => c.Email == _email && 
                          (_clienteId.HasValue && c.IdCliente != _clienteId.Value);
            }
        }
    }
}
