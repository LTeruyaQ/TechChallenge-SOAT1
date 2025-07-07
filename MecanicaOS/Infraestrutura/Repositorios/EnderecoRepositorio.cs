using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios;

public class EnderecoRepositorio : ICrudRepositorio<Endereco>
{
    private readonly MecanicaContexto _dbContext;

    public EnderecoRepositorio(MecanicaContexto dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Endereco> CadastrarAsync(Endereco endereco)
    {
        var entidade = await Task.Run(() => _dbContext.Enderecos.Add(endereco));
        return entidade.Entity;
    }

    public async Task DeletarAsync(Endereco endereco)
    {
        await Task.Run(() => _dbContext.Enderecos.Remove(endereco));
    }

    public async Task EditarAsync(Endereco endereco)
    {
        await Task.Run(() => _dbContext.Enderecos.Update(endereco));
    }

    public async Task<IEnumerable<Endereco>> ObterPorFiltroAsync(IEspecificacao<Endereco> endereco)
    {
        return await _dbContext.Enderecos.AsNoTracking().Where(endereco.Expressao).ToListAsync();
    }

    public async Task<Endereco?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Enderecos.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Endereco>> ObterTodosAsync()
    {
        return await _dbContext.Enderecos.AsNoTracking().ToListAsync();
    }

    public async Task<Endereco?> ObterUmAsync(IEspecificacao<Endereco> endereco)
    {
        return await _dbContext.Enderecos.AsNoTracking().Where(endereco.Expressao).SingleOrDefaultAsync();
    }
}
