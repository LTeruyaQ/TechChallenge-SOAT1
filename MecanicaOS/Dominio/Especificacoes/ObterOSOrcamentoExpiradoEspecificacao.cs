using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterOSOrcamentoExpiradoEspecificacao : EspecificacaoBase<OrdemServico>
{
    public override Expression<Func<OrdemServico, bool>> Expressao =>
        o => o.Status == StatusOrdemServico.AguardandoAprovação &&
             o.DataEnvioOrcamento.HasValue &&
             o.DataEnvioOrcamento.Value.AddDays(3) <= DateTime.UtcNow;

    public override List<Func<IQueryable<OrdemServico>, IQueryable<OrdemServico>>> Inclusoes =>
    [
        o => o.Include(o => o.InsumosOS).ThenInclude(ios => ios.Estoque)
    ];
}