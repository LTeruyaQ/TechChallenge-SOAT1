using Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Gateways
{
    public interface IClienteGateway
    {
        Task<Cliente> CadastrarAsync(Cliente cliente);
        Task DeletarAsync(Cliente cliente);
        Task EditarAsync(Cliente cliente);
        Task<Cliente?> ObterClienteComVeiculoPorIdAsync(Guid clienteId);
        Task<Cliente?> ObterClientePorDocumentoAsync(string documento);
        Task<Cliente?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Cliente>> ObterTodosClienteComVeiculoAsync();
    }
}
