using Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Gateways
{
    public interface IEstoqueGateway
    {
        Task CadastrarAsync(Estoque estoque);
        Task DeletarAsync(Estoque estoque);
        Task EditarAsync(Estoque estoque);
        Task<IEnumerable<Estoque>> ObterEstoqueCritico();
        Task<Estoque?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Estoque>> ObterTodosAsync();
    }
}
