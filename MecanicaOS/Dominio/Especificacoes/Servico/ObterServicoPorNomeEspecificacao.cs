using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Servico
{
    public class ObterServicoPorNomeEspecificacao : EspecificacaoBase<Entidades.Servico>
    {
        private string nome;

        public ObterServicoPorNomeEspecificacao(string nome)
        {
            this.nome = nome;
        }

        public override Expression<Func<Entidades.Servico, bool>> Expressao => s => s.Nome == nome;
    }
}