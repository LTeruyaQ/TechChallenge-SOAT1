using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.OrdemServico;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class OrdemServicoGateway : IOrdemServicoGateway
    {
        private readonly IRepositorio<OrdemServicoEntityDto> _repositorioOrdemServico;

        public OrdemServicoGateway(IRepositorio<OrdemServicoEntityDto> repositorioOrdemServico)
        {
            _repositorioOrdemServico = repositorioOrdemServico;
        }

        public async Task<OrdemServico> CadastrarAsync(OrdemServico ordemServico)
        {
            var dto = ToDto(ordemServico);
            var result = await _repositorioOrdemServico.CadastrarAsync(dto);
            return FromDto(result);
        }

        public async Task EditarAsync(OrdemServico ordemServico)
        {
            await _repositorioOrdemServico.EditarAsync(ToDto(ordemServico));
        }

        public async Task EditarVariosAsync(IEnumerable<OrdemServico> ordensServico)
        {
            var dtos = ordensServico.Select(ToDto);
            await _repositorioOrdemServico.EditarVariosAsync(dtos);
        }

        public async Task<IEnumerable<OrdemServico>> ListarOSOrcamentoExpiradoAsync()
        {
            var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();
            return await _repositorioOrdemServico.ListarProjetadoAsync<OrdemServico>(especificacao);
        }

        public async Task<OrdemServico?> ObterOrdemServicoPorIdComInsumos(Guid id)
        {
            var especificacao = new ObterOrdemServicoPorIdComInsumosEspecificacao(id);
            return await _repositorioOrdemServico.ObterUmProjetadoSemRastreamentoAsync<OrdemServico>(especificacao);
        }

        public async Task<IEnumerable<OrdemServico>> ObterOrdemServicoPorStatusAsync(StatusOrdemServico status)
        {
            var especificacao = new ObterOrdemServicoPorStatusEspecificacao(status);
            return await _repositorioOrdemServico.ListarProjetadoAsync<OrdemServico>(especificacao);
        }

        public async Task<OrdemServico?> ObterPorIdAsync(Guid id)
        {
            var dto = await _repositorioOrdemServico.ObterPorIdAsync(id);
            return dto != null ? FromDto(dto) : null;
        }

        public async Task<IEnumerable<OrdemServico>> ObterTodosAsync()
        {
            var dtos = await _repositorioOrdemServico.ObterTodosAsync();
            return dtos.Select(FromDto);
        }

        private static OrdemServicoEntityDto ToDto(OrdemServico ordemServico)
        {
            return new OrdemServicoEntityDto
            {
                Id = ordemServico.Id,
                Ativo = ordemServico.Ativo,
                DataCadastro = ordemServico.DataCadastro,
                DataAtualizacao = ordemServico.DataAtualizacao,
                ClienteId = ordemServico.ClienteId,
                VeiculoId = ordemServico.VeiculoId,
                ServicoId = ordemServico.ServicoId,
                Orcamento = ordemServico.Orcamento,
                DataEnvioOrcamento = ordemServico.DataEnvioOrcamento,
                Descricao = ordemServico.Descricao,
                Status = ordemServico.Status,
                InsumosOS = ordemServico.InsumosOS.Select(insumo => new InsumoOSEntityDto
                {
                    Id = insumo.Id,
                    Ativo = insumo.Ativo,
                    DataCadastro = insumo.DataCadastro,
                    DataAtualizacao = insumo.DataAtualizacao,
                    OrdemServicoId = insumo.OrdemServicoId,
                    EstoqueId = insumo.EstoqueId,
                    Quantidade = insumo.Quantidade
                }).ToList()
            };
        }

        private static OrdemServico FromDto(OrdemServicoEntityDto dto)
        {
            return new OrdemServico
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                ClienteId = dto.ClienteId,
                VeiculoId = dto.VeiculoId,
                ServicoId = dto.ServicoId,
                Orcamento = dto.Orcamento,
                DataEnvioOrcamento = dto.DataEnvioOrcamento,
                Descricao = dto.Descricao,
                Status = dto.Status,
                InsumosOS = dto.InsumosOS.Select(insumoDto => new InsumoOS
                {
                    Id = insumoDto.Id,
                    Ativo = insumoDto.Ativo,
                    DataCadastro = insumoDto.DataCadastro,
                    DataAtualizacao = insumoDto.DataAtualizacao,
                    OrdemServicoId = insumoDto.OrdemServicoId,
                    EstoqueId = insumoDto.EstoqueId,
                    Quantidade = insumoDto.Quantidade
                }).ToList()
            };
        }
    }
}
