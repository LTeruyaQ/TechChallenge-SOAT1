using Core.DTOs.Repositories.Estoque;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Estoque;

public class ObterEstoqueCriticoEspecificacao : EspecificacaoBase<EstoqueRepositoryDto>
{
    public ObterEstoqueCriticoEspecificacao()
    {
        DefinirProjecao(e => new Entidades.Estoque()
        {
            Id = e.Id,
            Ativo = e.Ativo,
            DataCadastro = e.DataCadastro,
            DataAtualizacao = e.DataAtualizacao,
            QuantidadeMinima = e.QuantidadeMinima,
            QuantidadeDisponivel = e.QuantidadeDisponivel,
            Descricao = e.Descricao,
        });
    }

    public override Expression<Func<EstoqueRepositoryDto, bool>> Expressao =>
         e => e.QuantidadeDisponivel <= e.QuantidadeMinima;
}