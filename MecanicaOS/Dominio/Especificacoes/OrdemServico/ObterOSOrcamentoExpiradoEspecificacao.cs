using Dominio.Enumeradores;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.OrdemServico;

public class ObterOSOrcamentoExpiradoEspecificacao : EspecificacaoBase<Entidades.OrdemServico>
{
    public ObterOSOrcamentoExpiradoEspecificacao()
    {
        AdicionarInclusao(os => os.Orcamento);
        AdicionarInclusao(os => os.InsumosOS, io => io.Estoque);
    }

    public override Expression<Func<Entidades.OrdemServico, bool>> Expressao =>
        o => o.Status == StatusOrdemServico.AguardandoAprovação && 
             o.OrcamentoId.HasValue &&
             o.Orcamento.DataEnvio.HasValue && 
             o.Orcamento.DataEnvio.Value.AddDays(o.Orcamento.DIAS_PARA_EXPIRACAO) <= DateTime.UtcNow;
}