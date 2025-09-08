using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.OrdemServico;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class OrdemServicoGateway : IOrdemServicoGateway
    {
        private readonly IRepositorio<OrdemServico> _repositorioOrdemServico;

        public OrdemServicoGateway(IRepositorio<OrdemServico> repositorioOrdemServico)
        {
            _repositorioOrdemServico = repositorioOrdemServico;
        }

        public async Task<OrdemServico> CadastrarAsync(OrdemServico ordemServico)
        {
            return await _repositorioOrdemServico.CadastrarAsync(ordemServico);
        }

        public async Task EditarAsync(OrdemServico ordemServico)
        {
            await _repositorioOrdemServico.EditarAsync(ordemServico);
        }

        public async Task EditarVariosAsync(IEnumerable<OrdemServico> ordensServico)
        {
            await _repositorioOrdemServico.EditarVariosAsync(ordensServico);
        }

        public async Task<IEnumerable<OrdemServico>> ListarOSOrcamentoExpiradoAsync()
        {
            var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();
            return await _repositorioOrdemServico.ListarAsync(especificacao);
        }

        public async Task<OrdemServico?> ObterOrdemServicoPorIdComInsumos(Guid id)
        {
            var especificacao = new ObterOrdemServicoPorIdComInsumosEspecificacao(id);
            return await _repositorioOrdemServico.ObterUmSemRastreamentoAsync(especificacao);
        }

        public async Task<IEnumerable<OrdemServico>> ObterOrdemServicoPorStatusAsync(StatusOrdemServico status)
        {
            var especificacao = new ObterOrdemServicoPorStatusEspecificacao(status);
            return await _repositorioOrdemServico.ListarAsync(especificacao);
        }

        public async Task<OrdemServico?> ObterPorIdAsync(Guid id)
        {
            return await _repositorioOrdemServico.ObterPorIdAsync(id);
        }

        public async Task<IEnumerable<OrdemServico>> ObterTodosAsync()
        {
            return await _repositorioOrdemServico.ObterTodosAsync();
        }
    }
}
