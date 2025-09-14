using Core.DTOs.Repositories.OrdemServicos;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.OrdemServico;

public class ObterOrdemServicoPorIdComIncludeEspecificacao : EspecificacaoBase<OrdemServicoRepositoryDto>
{
    private readonly Guid _id;

    public ObterOrdemServicoPorIdComIncludeEspecificacao(Guid id)
    {
        _id = id;
        AdicionarInclusao(os => os.Servico);
        AdicionarInclusao(os => os.Veiculo);
        AdicionarInclusao(os => os.Cliente.Contato);
        AdicionarInclusao(os => os.InsumosOS, io => io.Estoque);
    }

    public override Expression<Func<OrdemServicoRepositoryDto, bool>> Expressao => os => os.Id == _id;
}
