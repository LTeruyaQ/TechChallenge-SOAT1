using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios;

public class UsuarioRepositorio : ICrudRepositorio<Usuario>
{
    private readonly MecanicaContexto _dbContext;

    public UsuarioRepositorio(MecanicaContexto dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Usuario> CadastrarAsync(Usuario usuario)
    {
        var entidade = await Task.Run(() => _dbContext.Usuarios.Add(usuario));
        return entidade.Entity;
    }

    public async Task DeletarAsync(Usuario usuario)
    {
        await Task.Run(() => _dbContext.Usuarios.Remove(usuario));
    }

    public async Task EditarAsync(Usuario usuario)
    {
        await Task.Run(() => _dbContext.Usuarios.Update(usuario));
    }

    public async Task<IEnumerable<Usuario>> ObterPorFiltroAsync(IEspecificacao<Usuario> usuario)
    {
        return await _dbContext.Usuarios.AsNoTracking().Where(usuario.Expressao).ToListAsync();
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Usuarios.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Usuario>> ObterTodosAsync()
    {
        return await _dbContext.Usuarios.AsNoTracking().ToListAsync();
    }

    public async Task<Usuario?> ObterUmAsync(IEspecificacao<Usuario> usuario)
    {
        return await _dbContext.Usuarios.AsNoTracking().Where(usuario.Expressao).SingleOrDefaultAsync();
    }
}
