using Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Gateways
{
    public interface IContatoGateway
    {
        Task CadastrarAsync(Contato contato);
        Task EditarAsync(Contato contato);
        Task<Contato?> ObterPorIdAsync(Guid contatoId);
    }
}
