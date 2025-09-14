using Core.DTOs.Repositories.Servico;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Servico
{
    public class ObterServicoPorNomeEspecificacao : EspecificacaoBase<ServicoRepositoryDto>
    {
        private string nome;

        public ObterServicoPorNomeEspecificacao(string nome)
        {
            this.nome = nome;
        }

        public override Expression<Func<ServicoRepositoryDto, bool>> Expressao => s => s.Nome == nome;
    }
}