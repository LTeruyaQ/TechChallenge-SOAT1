using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes
{
    public class ObterServicoPorNomeEspecificacao : EspecificacaoBase<Servico>
    {
        private string nome;

        public ObterServicoPorNomeEspecificacao(string nome)
        {
            this.nome = nome;
        }

        public override Expression<Func<Servico, bool>> Expressao => s => s.Nome == nome;
    }
}