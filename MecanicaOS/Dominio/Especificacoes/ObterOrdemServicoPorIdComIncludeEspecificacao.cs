using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ObterOrdemServicoPorIdComIncludeEspecificacao : EspecificacaoBase<OrdemServico>
{
    private readonly Guid _id;

    public ObterOrdemServicoPorIdComIncludeEspecificacao(Guid id)
    {
        _id = id;
        AdicionarInclusao(os => os.Servico);
        AdicionarInclusao(os => os.Cliente.Contato);
        AdicionarInclusao(os => os.InsumosOS, io => io.Estoque);
    }

    public override Expression<Func<OrdemServico, bool>> Expressao => os => os.Id == _id;
}
