using Core.Entidades;
using Core.Especificacoes.Insumo;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class InsumosGateway : IInsumosGateway
    {
        private readonly IRepositorio<InsumoOS> _repositorioInsumoOS;

        public InsumosGateway(IRepositorio<InsumoOS> repositorioInsumoOS)
        {
            _repositorioInsumoOS = repositorioInsumoOS;
        }

        public async Task<IEnumerable<InsumoOS>> CadastrarVariosAsync(IEnumerable<InsumoOS> insumosOS)
        {
            return await _repositorioInsumoOS.CadastrarVariosAsync(insumosOS);
        }

        public async Task<IEnumerable<InsumoOS>> ObterInsumosOSPorOSAsync(Guid ordemServicoId)
        {
            var especificacao = new ObterInsumosOSPorOSEspecificacao(ordemServicoId);
            return await _repositorioInsumoOS.ListarAsync(especificacao);
        }
    }
}
