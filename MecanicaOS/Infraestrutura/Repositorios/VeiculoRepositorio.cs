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
        var entidade = await Task.Run(() => _dbContext.Veiculos.Add(veiculo));
        return entidade.Entity;
    }

    public async Task DeletarAsync(Veiculo veiculo)
    {
        await Task.Run(() => _dbContext.Veiculos.Remove(veiculo));
    }

    public async Task EditarAsync(Veiculo novaEntidade)
    {
        await Task.Run(() => _dbContext.Veiculos.Update(novaEntidade));
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
