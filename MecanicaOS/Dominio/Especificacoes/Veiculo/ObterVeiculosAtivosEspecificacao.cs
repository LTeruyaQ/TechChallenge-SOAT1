using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Veiculo
{
    public class ObterVeiculosAtivosEspecificacao : EspecificacaoBase<Entidades.Veiculo>
    {
        public ObterVeiculosAtivosEspecificacao()
        {
            AdicionarInclusao(v => v.Cliente);
        }

        public override Expression<Func<Entidades.Veiculo, bool>> Expressao =>
            v => v.Ativo;
    }
}
