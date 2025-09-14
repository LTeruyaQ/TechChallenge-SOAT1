using Core.DTOs.Repositories.Estoque;
using Core.DTOs.Repositories.OrdemServicos;
using Core.Entidades;
using Core.Especificacoes.Estoque;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class AlertaEstoqueGateway : IAlertaEstoqueGateway
    {
        private readonly IRepositorio<AlertaEstoqueRepositoryDto> _repositorioAlertaEstoque;

        public AlertaEstoqueGateway(IRepositorio<AlertaEstoqueRepositoryDto> repositorioAlertaEstoque)
        {
            _repositorioAlertaEstoque = repositorioAlertaEstoque;
        }

        public async Task CadastrarVariosAsync(IEnumerable<AlertaEstoque> alertas)
        {
            await _repositorioAlertaEstoque.CadastrarVariosAsync(alertas.Select(ToDto));
        }

        private static AlertaEstoqueRepositoryDto ToDto(AlertaEstoque alerta)
        {
            return new AlertaEstoqueRepositoryDto
            {
                Id = alerta.Id,
                Ativo = alerta.Ativo,
                DataCadastro = alerta.DataCadastro,
                DataAtualizacao = alerta.DataAtualizacao,
                EstoqueId = alerta.EstoqueId,
                Estoque = alerta.Estoque == null ? null : new EstoqueRepositoryDto
                {
                    Id = alerta.Estoque.Id,
                    Ativo = alerta.Estoque.Ativo,
                    DataCadastro = alerta.Estoque.DataCadastro,
                    DataAtualizacao = alerta.Estoque.DataAtualizacao,
                    QuantidadeDisponivel = alerta.Estoque.QuantidadeDisponivel,
                    QuantidadeMinima = alerta.Estoque.QuantidadeMinima,
                    Descricao = alerta.Estoque.Descricao,
                    Insumo = alerta.Estoque.Insumo,
                    Preco = alerta.Estoque.Preco
                }
            };
        }

        public async Task<IEnumerable<AlertaEstoque>> ObterAlertaDoDiaPorEstoqueAsync(Guid insumoId, DateTime dataAtual)
        {
            return await _repositorioAlertaEstoque.ListarProjetadoAsync<AlertaEstoque>(
                new ObterAlertaDoDiaPorEstoqueEspecificacao(
                    insumoId,
                    dataAtual));
        }
    }
}
