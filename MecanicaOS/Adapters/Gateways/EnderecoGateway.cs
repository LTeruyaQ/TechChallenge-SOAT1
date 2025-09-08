using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class EnderecoGateway : IEnderecoGateway
    {
        private readonly IRepositorio<Endereco> _repositorioEndereco;

        public EnderecoGateway(IRepositorio<Endereco> repositorioEndereco)
        {
            _repositorioEndereco = repositorioEndereco;
        }

        public async Task CadastrarAsync(Endereco endereco)
        {
            await _repositorioEndereco.CadastrarAsync(endereco);
        }

        public async Task EditarAsync(Endereco endereco)
        {
            await _repositorioEndereco.EditarAsync(endereco);
        }

        public async Task<Endereco?> ObterPorIdAsync(Guid enderecoId)
        {
            return await _repositorioEndereco.ObterPorIdAsync(enderecoId);
        }
    }
}
