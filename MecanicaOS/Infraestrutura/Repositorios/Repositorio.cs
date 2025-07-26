using Dominio.Entidades.Abstratos;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Infraestrutura.Dados.Especificacoes;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios
{
    public class Repositorio<T> : IRepositorio<T> where T : Entidade
    {
        protected readonly MecanicaContexto _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repositorio(MecanicaContexto dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<T> CadastrarAsync(T entidade)
        {
            var resultado = await _dbSet.AddAsync(entidade);
            return resultado.Entity;
        }

        public virtual async Task<IEnumerable<T>> CadastrarVariosAsync(IEnumerable<T> entidades)
        {
            if (entidades == null || !entidades.Any())
                return Enumerable.Empty<T>();

            await _dbSet.AddRangeAsync(entidades);
            return entidades;
        }

        public virtual async Task DeletarAsync(T entidade)
        {
            await Task.Run(() => _dbSet.Remove(entidade));
        }

        public virtual async Task DeletarVariosAsync(IEnumerable<T> entidades)
        {
            foreach (var entidade in entidades)
            {
                await Task.Run(() => _dbSet.Remove(entidade));
            }
        }

        public virtual async Task DeletarLogicamenteAsync(T entidade)
        {
            await Task.Run(() => entidade.Ativo = false);
        }

        public virtual async Task EditarAsync(T entidade)
        {
            await Task.Run(() => _dbContext.Entry(entidade).State = EntityState.Modified);
        }

        public virtual async Task EditarVariosAsync(IEnumerable<T> entidades)
        {
            if (entidades == null || !entidades.Any())
                return;

            foreach (var entidade in entidades)
            {
                await Task.Run(() => _dbContext.Entry(entidade).State = EntityState.Modified);
            }
        }

        public virtual async Task<IEnumerable<T>> ObterPorFiltroAsync(IEspecificacao<T> especificacao)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet, especificacao);
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ObterPorFiltroSemRastreamentoAsync(IEspecificacao<T> especificacao)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet.AsNoTracking(), especificacao);
            return await query.ToListAsync();
        }

        public virtual async Task<T?> ObterPorIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<IEnumerable<T>> ObterTodosAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<T?> ObterUmSemRastreamentoAsync(IEspecificacao<T> especificacao)
        {
            return await AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet.AsNoTracking(), especificacao)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<T?> ObterUmAsync(IEspecificacao<T> especificacao)
        {
            return await AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet, especificacao)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> ObterPorFiltroPaginadoSemRastreamentoAsync(IEspecificacao<T> especificacao, int pagina = 0, int tamanho = 20)
        {
            return await AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet.AsNoTracking(), especificacao)
                .Skip(pagina * tamanho)
                .Take(tamanho)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterPorFiltroPaginadoAsync(IEspecificacao<T> especificacao, int pagina = 0, int tamanho = 20)
        {
            return await AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet, especificacao)
                .Skip(pagina * tamanho)
                .Take(tamanho)
                .ToListAsync();
        }

        public virtual async Task<TProjecao?> ObterProjetadoAsync<TProjecao>(
            IEspecificacao<T> especificacao,
            CancellationToken cancellationToken = default)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet, especificacao);
            return await AvaliadorDeEspecificacao<T>
                .AplicarProjecao<TProjecao>(query, especificacao)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TProjecao?> ObterProjetadoSemRastreamentoAsync<TProjecao>(
            IEspecificacao<T> especificacao,
            CancellationToken cancellationToken = default)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet.AsNoTracking(), especificacao);
            return await AvaliadorDeEspecificacao<T>
                .AplicarProjecao<TProjecao>(query, especificacao)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TProjecao>> ListarProjetadoAsync<TProjecao>(
            IEspecificacao<T> especificacao,
            int pagina = 0,
            int tamanho = 20,
            CancellationToken cancellationToken = default)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet, especificacao);
            return await AvaliadorDeEspecificacao<T>
                .AplicarProjecao<TProjecao>(query, especificacao)
                .Skip(pagina * tamanho)
                .Take(tamanho)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TProjecao>> ListarProjetadoSemRastreamentoAsync<TProjecao>(
            IEspecificacao<T> especificacao,
            int pagina = 0,
            int tamanho = 20,
            CancellationToken cancellationToken = default)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet.AsNoTracking(), especificacao);
            return await AvaliadorDeEspecificacao<T>
                .AplicarProjecao<TProjecao>(query, especificacao)
                .Skip(pagina * tamanho)
                .Take(tamanho)
                .ToListAsync(cancellationToken);
        }
    }
}