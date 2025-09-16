using Core.DTOs.Entidades.Servico;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Servico
{
    public class ObterServicoDisponivelEspecificacao : EspecificacaoBase<ServicoEntityDto>
    {
        public override Expression<Func<ServicoEntityDto, bool>> Expressao => s => s.Disponivel;
    }
}
