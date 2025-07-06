using System.Linq;
using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios;

public class ClienteRepositorio : ICrudRepositorio<Cliente>
{
    private readonly MecanicaContexto _dbContext;

    public ClienteRepositorio(MecanicaContexto dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cliente> CadastrarAsync(Cliente cliente)
    {
        var entidade = await Task.Run(() => _dbContext.Clientes.Add(cliente));
        return entidade.Entity;
    }

    public async Task DeletarAsync(Cliente cliente)
    {
        await Task.Run(() => _dbContext.Clientes.Remove(cliente));
    }

    public async Task EditarAsync(Cliente cliente)
    {
        await Task.Run(() => _dbContext.Clientes.Update(cliente));
    }

    public async Task<IEnumerable<Cliente>> ObterPorFiltroAsync(IEspecificacao<Cliente> cliente)
    {
        return await _dbContext.Clientes.AsNoTracking().Where(cliente.Expressao).ToListAsync();
    }

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Clientes.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Cliente>> ObterTodosAsync()
    {
        return await _dbContext.Clientes.AsNoTracking().ToListAsync();
    }

    public async Task<Cliente?> ObterUmAsync(IEspecificacao<Cliente> cliente)
    {
        return await _dbContext.Clientes.AsNoTracking().Where(cliente.Expressao).SingleOrDefaultAsync();
    }
}
