using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios;

public class EstoqueRepositorio : ICrudRepositorio<Estoque>
{
    private readonly MecanicaContexto _dbContext;

    public EstoqueRepositorio(MecanicaContexto dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Estoque> CadastrarAsync(Estoque estoque)
    {
        var entidade =_dbContext.Estoques.Add(estoque);
        await _dbContext.SaveChangesAsync();
        return entidade.Entity;
    }

    public async Task DeletarAsync(Estoque estoque)
    {
        _dbContext.Estoques.Remove(estoque);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Editar(Estoque novaEntidade)
    {
        _dbContext.Estoques.Update(novaEntidade);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Estoque>> ObterPorFiltro(IEspecificacao<Estoque> filtro)
    {
        return await _dbContext.Estoques.AsNoTracking().Where(x => filtro.EhSatisfeitoPor(x)).ToListAsync();
    }

    public async Task<Estoque?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Estoques.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Estoque>> ObterTodos()
    {
        return await _dbContext.Estoques.AsNoTracking().ToListAsync();
    }
}
