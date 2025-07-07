using System.Linq.Expressions;
using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes
{
    public class ObterServicoPorNomeEspecificacao : IEspecificacao<Servico>
    {
        private string nome;

        public ObterServicoPorNomeEspecificacao(string nome)
        {
            this.nome = nome;
        }

        public Expression<Func<Servico, bool>> Expressao => s => s.Nome == nome;
    }
}