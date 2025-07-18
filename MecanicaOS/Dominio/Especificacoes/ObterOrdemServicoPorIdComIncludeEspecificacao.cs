using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterOrdemServicoPorIdComIncludeEspecificacao : EspecificacaoBase<OrdemServico>
{
    private readonly Guid _id;

    public ObterOrdemServicoPorIdComIncludeEspecificacao(Guid id) => _id = id;

    public override Expression<Func<OrdemServico, bool>> Expressao => os => os.Id == _id;

    public override List<Func<IQueryable<OrdemServico>, IQueryable<OrdemServico>>> Inclusoes =>
    [
        q => q.Include(os => os.Servico),
        q => q.Include(os => os.Cliente).ThenInclude(c => c.Contato),
        q => q.Include(os => os.InsumosOS).ThenInclude(io => io.Estoque),
    ];
}
