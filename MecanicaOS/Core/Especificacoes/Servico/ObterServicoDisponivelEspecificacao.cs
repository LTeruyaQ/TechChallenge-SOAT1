using Core.DTOs.Repositories.Servico;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Servico
{
    public class ObterServicoDisponivelEspecificacao : EspecificacaoBase<ServicoRepositoryDto>
    {
        public override Expression<Func<ServicoRepositoryDto, bool>> Expressao => s => s.Disponivel;
    }
}
