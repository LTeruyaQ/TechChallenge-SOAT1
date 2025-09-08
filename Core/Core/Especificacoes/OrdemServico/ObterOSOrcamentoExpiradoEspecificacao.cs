using Core.Enumeradores;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.OrdemServico;

public class ObterOSOrcamentoExpiradoEspecificacao : EspecificacaoBase<Entidades.OrdemServico>
{
    public ObterOSOrcamentoExpiradoEspecificacao()
    {
        AdicionarInclusao(os => os.InsumosOS, io => io.Estoque);
    }

    public override Expression<Func<Entidades.OrdemServico, bool>> Expressao =>
        o => o.Status == StatusOrdemServico.AguardandoAprovação &&
             o.DataEnvioOrcamento.HasValue &&
             o.DataEnvioOrcamento.Value.AddDays(3) <= DateTime.UtcNow;
}