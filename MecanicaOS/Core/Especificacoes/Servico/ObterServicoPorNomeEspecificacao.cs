using Core.DTOs.Entidades.Servico;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Servico
{
    public class ObterServicoPorNomeEspecificacao : EspecificacaoBase<ServicoEntityDto>
    {
        private string nome;

        public ObterServicoPorNomeEspecificacao(string nome)
        {
            this.nome = nome;
        }

        public override Expression<Func<ServicoEntityDto, bool>> Expressao => s => s.Nome == nome;
    }
}