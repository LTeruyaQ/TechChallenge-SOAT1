using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios
{
    public class VeiculoRepositorio : ICrudRepositorio<Veiculo>
    {
        private readonly MecanicaContexto _dbContext;

        public VeiculoRepositorio(MecanicaContexto dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Veiculo> CadastrarAsync(Veiculo Veiculo)
        {
            var entidade = _dbContext.Veiculos.Add(Veiculo);
            await _dbContext.SaveChangesAsync();
            return entidade.Entity;
        }

        public async Task DeletarAsync(Veiculo Veiculo)
        {
            _dbContext.Veiculos.Remove(Veiculo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Editar(Veiculo novoVeiculo)
        {
            _dbContext.Veiculos.Update(novoVeiculo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Veiculo>> ObterPorFiltro(IEspecificacao<Veiculo> especificacao)
        {
            return await _dbContext.Veiculos.AsNoTracking().Where(especificacao.Expressao).ToListAsync();
        }

        public async Task<Veiculo> ObterPorIdAsync(Guid id)
        {
            return await _dbContext.Veiculos.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Veiculo>> ObterTodos()
        {
            return await _dbContext.Veiculos.AsNoTracking().ToListAsync();
        }
    }
}
