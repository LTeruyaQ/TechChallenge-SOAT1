using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterOSOrcamentoExpiradoEspecificacao : EspecificacaoBase<OrdemServico>
{
    public ObterOSOrcamentoExpiradoEspecificacao()
    {
        AdicionarInclusao(os => os.InsumosOS, io => io.Estoque);
    }

    public override Expression<Func<OrdemServico, bool>> Expressao =>
        o => o.Status == StatusOrdemServico.AguardandoAprovação &&
             o.DataEnvioOrcamento.HasValue &&
             o.DataEnvioOrcamento.Value.AddDays(3) <= DateTime.UtcNow;
}