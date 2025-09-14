using Core.DTOs.Repositories.OrdemServicos;
using Core.Entidades;
using Core.Especificacoes.Insumo;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class InsumosGateway : IInsumosGateway
    {
        private readonly IRepositorio<InsumoOSRepositoryDto> _repositorioInsumoOS;

        public InsumosGateway(IRepositorio<InsumoOSRepositoryDto> repositorioInsumoOS)
        {
            _repositorioInsumoOS = repositorioInsumoOS;
        }

        public async Task<IEnumerable<InsumoOS>> CadastrarVariosAsync(IEnumerable<InsumoOS> insumosOS)
        {
            var insumosCadastrados = await _repositorioInsumoOS.CadastrarVariosAsync(insumosOS.Select(ToDto));
            return insumosCadastrados.Select(FromDto);
        }

        private static InsumoOSRepositoryDto ToDto(InsumoOS insumo)
        {
            return new InsumoOSRepositoryDto
            {
                Id = insumo.Id,
                Ativo = insumo.Ativo,
                DataCadastro = insumo.DataCadastro,
                DataAtualizacao = insumo.DataAtualizacao,
                OrdemServicoId = insumo.OrdemServicoId,
                EstoqueId = insumo.EstoqueId,
                Quantidade = insumo.Quantidade
            };
        }

        private static InsumoOS FromDto(InsumoOSRepositoryDto dto)
        {
            return new InsumoOS
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                OrdemServicoId = dto.OrdemServicoId,
                EstoqueId = dto.EstoqueId,
                Quantidade = dto.Quantidade
            };
        }

        public async Task<IEnumerable<InsumoOS>> ObterInsumosOSPorOSAsync(Guid ordemServicoId)
        {
            var especificacao = new ObterInsumosOSPorOSEspecificacao(ordemServicoId);
            return await _repositorioInsumoOS.ListarProjetadoAsync<InsumoOS>(especificacao);
        }
    }
}
