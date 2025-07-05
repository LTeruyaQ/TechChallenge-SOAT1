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
        var entidade = await Task.Run(() => _dbContext.Estoques.Add(estoque));
        return entidade.Entity;
    }

    public async Task DeletarAsync(Estoque estoque)
    {
        await Task.Run(() => _dbContext.Estoques.Remove(estoque));
    }

    public async Task EditarAsync(Estoque novaEntidade)
    {
        await Task.Run(() => _dbContext.Estoques.Update(novaEntidade));
    }

    public async Task<IEnumerable<Estoque>> ObterPorFiltroAsync(IEspecificacao<Estoque> especificacao)
    {
        return await _dbContext.Estoques.AsNoTracking().Where(especificacao.Expressao).ToListAsync();
    }

    public async Task<Estoque?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Estoques.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Estoque>> ObterTodosAsync()
    {
        return await _dbContext.Estoques.AsNoTracking().ToListAsync();
    }
}
