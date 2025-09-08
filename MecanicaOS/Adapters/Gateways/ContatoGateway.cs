using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class ContatoGateway : IContatoGateway
    {
        private readonly IRepositorio<Contato> _repositorioContato;

        public ContatoGateway(IRepositorio<Contato> repositorioContato)
        {
            _repositorioContato = repositorioContato;
        }

        public async Task CadastrarAsync(Contato contato)
        {
            await _repositorioContato.CadastrarAsync(contato);
        }

        public async Task EditarAsync(Contato contato)
        {
            await _repositorioContato.EditarAsync(contato);
        }

        public async Task<Contato?> ObterPorIdAsync(Guid contatoId)
        {
            return await _repositorioContato.ObterPorIdAsync(contatoId);
        }
    }
}
