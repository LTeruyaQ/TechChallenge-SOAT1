using Core.Entidades;
using Core.Especificacoes.Cliente;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class ClienteGateway : IClienteGateway
    {
        private readonly IRepositorio<Cliente> _repositorioCliente;

        public ClienteGateway(IRepositorio<Cliente> repositorioCliente)
        {
            _repositorioCliente = repositorioCliente;
        }

        public async Task<Cliente> CadastrarAsync(Cliente cliente)
        {
            return await _repositorioCliente.CadastrarAsync(cliente);
        }

        public async Task DeletarAsync(Cliente cliente)
        {
            await _repositorioCliente.DeletarAsync(cliente);
        }

        public async Task EditarAsync(Cliente cliente)
        {
            await _repositorioCliente.EditarAsync(cliente);
        }

        public async Task<Cliente?> ObterClienteComVeiculoPorIdAsync(Guid clienteId)
        {
            var especificacao = new ObterClienteComVeiculoPorIdEspecificacao(clienteId);
            return await _repositorioCliente.ObterUmSemRastreamentoAsync(especificacao);
        }

        public async Task<Cliente?> ObterClientePorDocumentoAsync(string documento)
        {
            var especificacao = new ObterClientePorDocumento(documento);
            return await _repositorioCliente.ObterUmSemRastreamentoAsync(especificacao);
        }

        public async Task<Cliente?> ObterPorIdAsync(Guid id)
        {
            return await _repositorioCliente.ObterPorIdAsync(id);
        }

        public async Task<IEnumerable<Cliente>> ObterTodosClienteComVeiculoAsync()
        {
            var especificacao = new ObterTodosClienteComVeiculoEspecificacao();
            return await _repositorioCliente.ListarAsync(especificacao);
        }
    }
}
