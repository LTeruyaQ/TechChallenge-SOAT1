using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios;

public class VeiculoRepositorio : ICrudRepositorio<Veiculo>
{
    private readonly MecanicaContexto _dbContext;

    public VeiculoRepositorio(MecanicaContexto dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Veiculo> CadastrarAsync(Veiculo veiculo)
    {
        var entidade = _dbContext.Veiculos.Add(veiculo);
        await _dbContext.SaveChangesAsync();
        return entidade.Entity;
    }

    public async Task DeletarAsync(Veiculo veiculo)
    {
        _dbContext.Veiculos.Remove(veiculo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditarAsync(Veiculo novaEntidade)
    {
        _dbContext.Veiculos.Update(novaEntidade);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Veiculo>> ObterPorFiltroAsync(IEspecificacao<Veiculo> especificacao)
    {
        return await _dbContext.Veiculos
            .AsNoTracking()
            .Where(especificacao.Expressao)
            .ToListAsync();
    }

    public async Task<Veiculo?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Veiculos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
    {
        return await _dbContext.Veiculos
            .AsNoTracking()
            .ToListAsync();
    }
}
