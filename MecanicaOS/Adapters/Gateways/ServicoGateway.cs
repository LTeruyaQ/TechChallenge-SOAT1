using Core.Entidades;
using Core.Especificacoes.Base.Extensoes;
using Core.Especificacoes.Servico;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class ServicoGateway : IServicoGateway
    {
        private readonly IRepositorio<Servico> _repositorioServico;

        public ServicoGateway(IRepositorio<Servico> repositorioServico)
        {
            _repositorioServico = repositorioServico;
        }

        public async Task<Servico> CadastrarAsync(Servico servico)
        {
            return await _repositorioServico.CadastrarAsync(servico);
        }

        public async Task DeletarAsync(Servico servico)
        {
            await _repositorioServico.DeletarAsync(servico);
        }

        public async Task EditarAsync(Servico servico)
        {
            await _repositorioServico.EditarAsync(servico);
        }

        public async Task<Servico?> ObterPorIdAsync(Guid id)
        {
            return await _repositorioServico.ObterPorIdAsync(id);
        }

        public async Task<IEnumerable<Servico>> ObterServicoDisponivelAsync()
        {
            ObterServicoDisponivelEspecificacao filtro = new ();

            return await _repositorioServico.ListarAsync(filtro); ;
        }

        public async Task<Servico?> ObterServicosDisponiveisPorNomeAsync(string nome)
        {
            var especificacao = new ObterServicoPorNomeEspecificacao(nome)
                .E(new ObterServicoDisponivelEspecificacao());

            return await _repositorioServico.ObterUmSemRastreamentoAsync(especificacao);
        }

        public async Task<IEnumerable<Servico>> ObterTodosAsync()
        {
            return await _repositorioServico.ObterTodosAsync();
        }
    }
}
